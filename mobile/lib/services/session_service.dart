import 'dart:convert';

import 'package:shared_preferences/shared_preferences.dart';

import '../models/user_profile.dart';

class SessionService {
  static const _isAuthenticatedKey = 'sleep-sanctuary-is-authenticated';
  static const _authTokenKey = 'sleep-sanctuary-auth-token';
  static const _currentUserKey = 'sleep-sanctuary-current-user';
  static const _themeKey = 'app-theme';

  Future<bool> isAuthenticated() async {
    final prefs = await SharedPreferences.getInstance();
    final authenticated = prefs.getBool(_isAuthenticatedKey) ?? false;
    final token = prefs.getString(_authTokenKey)?.trim() ?? '';
    final rawUser = prefs.getString(_currentUserKey)?.trim() ?? '';

    if (!authenticated || !_isValidToken(token) || rawUser.isEmpty) {
      if (authenticated || token.isNotEmpty || rawUser.isNotEmpty) {
        await logout();
      }
      return false;
    }

    try {
      final profile = UserProfile.fromJson(jsonDecode(rawUser) as Map<String, dynamic>);
      if (_isValidProfile(profile)) return true;
      await logout();
      return false;
    } catch (_) {
      await logout();
      return false;
    }
  }

  Future<String> getAuthToken() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_authTokenKey)?.trim() ?? '';
  }

  Future<void> saveAuthenticatedSession({
    required String accessToken,
    required UserProfile profile,
  }) async {
    final token = accessToken.trim();
    if (!_isValidToken(token)) {
      throw ArgumentError('Token de autenticacao ausente.');
    }
    if (!_isValidProfile(profile)) {
      throw ArgumentError('Perfil de usuario invalido.');
    }

    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_authTokenKey, token);
    await prefs.setBool(_isAuthenticatedKey, true);
    await prefs.setString(_currentUserKey, jsonEncode(profile.toJson()));
  }

  Future<void> saveCurrentUser(UserProfile profile) async {
    if (!_isValidProfile(profile)) {
      throw ArgumentError('Perfil de usuario invalido.');
    }
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_currentUserKey, jsonEncode(profile.toJson()));
  }

  Future<UserProfile?> getCurrentUser() async {
    final prefs = await SharedPreferences.getInstance();
    final raw = prefs.getString(_currentUserKey);
    if (raw == null || raw.isEmpty) return null;
    try {
      return UserProfile.fromJson(jsonDecode(raw) as Map<String, dynamic>);
    } catch (_) {
      return null;
    }
  }

  Future<void> logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_isAuthenticatedKey);
    await prefs.remove(_authTokenKey);
    await prefs.remove(_currentUserKey);
  }

  Future<String> getThemeModeName() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getString(_themeKey) ?? 'dark';
  }

  Future<void> saveThemeModeName(String value) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_themeKey, value);
  }

  bool _isValidProfile(UserProfile profile) {
    return profile.name.trim().isNotEmpty;
  }

  bool _isValidToken(String token) {
    final normalized = token.trim().toLowerCase();
    return normalized.isNotEmpty &&
        normalized != 'fake' &&
        normalized != 'mock' &&
        normalized != 'demo' &&
        normalized != 'guest' &&
        normalized != 'anonymous' &&
        normalized != 'null' &&
        normalized != 'undefined';
  }
}
