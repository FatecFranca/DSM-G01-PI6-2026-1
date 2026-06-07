import 'dart:async';
import 'dart:convert';
import 'dart:io' show Platform, SocketException;

import 'package:flutter/foundation.dart';
import 'package:http/http.dart' as http;

import '../models/insights.dart';
import '../models/sleep_record.dart';
import '../models/user_profile.dart';
import 'session_service.dart';

class ApiException implements Exception {
  const ApiException(this.message);

  final String message;

  @override
  String toString() => message;
}

const backendUnavailableMessage =
    'Não foi possível conectar ao servidor. O backend pode estar fora do ar ou inacessível no momento. Verifique sua conexão e tente novamente.';

class ApiService {
  ApiService({http.Client? client, SessionService? session, String? baseUrl})
    : _client = client ?? http.Client(),
      _session = session ?? SessionService(),
      baseUrl = baseUrl ?? defaultBaseUrl;

  final http.Client _client;
  final SessionService _session;
  final String baseUrl;

  static String get defaultBaseUrl {
    const configured = String.fromEnvironment('API_BASE_URL');
    if (configured.isNotEmpty) return configured;
    if (kIsWeb) return 'http://34.171.246.210:8080/';
    if (defaultTargetPlatform == TargetPlatform.android) {
      return 'http://34.171.246.210:8080/';
    }
    try {
      if (Platform.isAndroid) return 'http://10.0.2.2:8000';
    } catch (_) {
      return 'http://34.171.246.210:8080/';
    }
    return 'http://34.171.246.210:8080/';
  }

  Future<LoginResult> login(String email, String password) async {
    _debugLog('Login iniciado para email=$email');
    final json = await _request(
      '/login',
      method: 'POST',
      auth: false,
      body: {'email': email, 'password': password},
    );

    if (json is! Map<String, dynamic>) {
      _debugLog('Login bloqueado: resposta nao veio como objeto JSON.');
      throw const ApiException('Resposta de login invalida.');
    }

    final accessToken = _extractAccessToken(json);
    if (accessToken.isEmpty) {
      _debugLog('Login bloqueado: resposta de sucesso sem token esperado.');
      throw const ApiException('Resposta de login sem token de acesso.');
    }

    _debugLog('Login permitido: backend retornou token valido.');
    return LoginResult(accessToken: accessToken);
  }

  Future<LoginResult> registerUser(Map<String, dynamic> data) async {
    final json = await _request(
      '/user',
      method: 'POST',
      auth: false,
      body: data,
    );

    if (json is! Map<String, dynamic>) {
      _debugLog('Cadastro bloqueado: resposta nao veio como objeto JSON.');
      throw const ApiException('Resposta de cadastro invalida.');
    }

    final accessToken = _extractAccessToken(json);
    if (accessToken.isEmpty) {
      _debugLog('Cadastro bloqueado: resposta de sucesso sem token esperado.');
      throw const ApiException('Resposta de cadastro sem token de acesso.');
    }

    return LoginResult(
      accessToken: accessToken,
      name: (json['name'] ?? json['Name'])?.toString(),
    );
  }

  Future<void> saveSleepRecord(SleepRecord record) async {
    await _request('/sleep', method: 'POST', body: record.toApiJson());
  }

  Future<List<SleepRecord>> getSleepHistory() async {
    final json = await _request('/sleep');
    if (json is List) {
      return json
          .whereType<Map<String, dynamic>>()
          .map(SleepRecord.fromJson)
          .toList();
    }
    if (json is Map && json['items'] is List) {
      return (json['items'] as List)
          .whereType<Map<String, dynamic>>()
          .map(SleepRecord.fromJson)
          .toList();
    }
    return const [];
  }

  Future<Insights> getInsights() async {
    final json = await _request('/insights');
    return Insights.fromJson(json as Map<String, dynamic>);
  }

  Future<UserProfile> getUserProfile({String? accessToken}) async {
    final json = await _request('/user', accessToken: accessToken);
    return UserProfile.fromJson(json as Map<String, dynamic>);
  }

  Future<dynamic> _request(
    String endpoint, {
    String method = 'GET',
    bool auth = true,
    Map<String, dynamic>? body,
    String? accessToken,
  }) async {
    final normalizedBaseUrl = baseUrl.endsWith('/')
        ? baseUrl.substring(0, baseUrl.length - 1)
        : baseUrl;
    final normalizedEndpoint = endpoint.startsWith('/')
        ? endpoint
        : '/$endpoint';
    final uri = Uri.parse('$normalizedBaseUrl$normalizedEndpoint');
    final token = auth ? (accessToken?.trim() ?? await _session.getAuthToken()) : '';
    final headers = {
      'Content-Type': 'application/json',
      if (token.isNotEmpty) 'Authorization': 'Bearer $token',
    };

    if (auth && token.isEmpty) {
      _debugLog(
        'Chamada bloqueada antes do HTTP: rota protegida sem token. '
        'method=$method endpoint=$normalizedEndpoint',
      );
      throw const ApiException('Sessao expirada. Faca login novamente.');
    }

    _debugLog('HTTP request: $method $uri');
    _debugLog('HTTP endpoint: $normalizedEndpoint');
    _debugLog('HTTP headers: ${_safeJsonEncode(_sanitizeHeaders(headers))}');
    if (body != null) {
      _debugLog('HTTP payload: ${_safeJsonEncode(_sanitizeValue(body))}');
    }

    late http.Response response;
    try {
      late Future<http.Response> request;
      switch (method) {
        case 'POST':
          request = _client.post(
            uri,
            headers: headers,
            body: jsonEncode(body),
          );
          break;
        case 'PUT':
          request = _client.put(
            uri,
            headers: headers,
            body: jsonEncode(body),
          );
          break;
        default:
          request = _client.get(uri, headers: headers);
      }
      response = await request.timeout(const Duration(seconds: 15));
    } on http.ClientException catch (error) {
      _debugLog(
        'HTTP ClientException em $method $uri. '
        'Possivel erro de rede/CORS no Flutter Web. error=$error',
      );
      throw const ApiException(backendUnavailableMessage);
    } on TimeoutException catch (error) {
      _debugLog('HTTP timeout em $method $uri: $error');
      throw const ApiException(backendUnavailableMessage);
    } on SocketException catch (error) {
      _debugLog('HTTP SocketException em $method $uri: $error');
      throw const ApiException(backendUnavailableMessage);
    } catch (error) {
      _debugLog('HTTP erro de rede em $method $uri: $error');
      throw const ApiException(backendUnavailableMessage);
    }

    _debugLog(
      'HTTP status: ${response.statusCode} para $method $normalizedEndpoint',
    );
    _debugLog('HTTP response body: ${_sanitizeResponseBody(response.body)}');

    if (response.statusCode < 200 || response.statusCode >= 300) {
      if (response.statusCode == 400 ||
          response.statusCode == 401 ||
          response.statusCode == 403) {
        _debugLog(
          'HTTP bloqueado pelo backend: status=${response.statusCode} '
          'endpoint=$normalizedEndpoint',
        );
      }
      throw ApiException(_extractError(response));
    }

    if (response.body.trim().isEmpty) return null;
    try {
      return jsonDecode(response.body);
    } on FormatException catch (error) {
      _debugLog(
        'Erro de parsing JSON em $method $normalizedEndpoint: $error. '
        'body=${_sanitizeResponseBody(response.body)}',
      );
      throw const ApiException('Resposta invalida do backend.');
    }
  }

  String _extractError(http.Response response) {
    try {
      final decoded = jsonDecode(response.body);
      if (decoded is Map && decoded['errors'] is List) {
        return (decoded['errors'] as List).join(', ');
      }
      if (decoded is Map && decoded['message'] != null) {
        return decoded['message'].toString();
      }
      return decoded.toString();
    } catch (_) {
      return response.body.isNotEmpty
          ? response.body
          : 'Erro ${response.statusCode} ao chamar a API.';
    }
  }

  String _extractAccessToken(Map<String, dynamic> payload) {
    final directToken = payload['accessToken'] ?? payload['AccessToken'];
    if (directToken is String && _isUsableAccessToken(directToken)) {
      return directToken.trim();
    }

    final nestedToken = payload['tokens'] ?? payload['Tokens'];
    if (nestedToken is Map) {
      final token = nestedToken['accessToken'] ?? nestedToken['AccessToken'];
      if (token is String && _isUsableAccessToken(token)) {
        return token.trim();
      }
    }

    return '';
  }

  void _debugLog(String message) {
    debugPrint('[ApiService] $message');
  }

  bool _isUsableAccessToken(String token) {
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

  Map<String, String> _sanitizeHeaders(Map<String, String> headers) {
    return headers.map((key, value) {
      if (key.toLowerCase() == 'authorization') {
        return MapEntry(key, _maskSecret(value));
      }
      return MapEntry(key, value);
    });
  }

  dynamic _sanitizeValue(dynamic value) {
    if (value is Map) {
      return value.map((key, item) {
        final normalizedKey = key.toString().toLowerCase();
        if (_isSensitiveKey(normalizedKey)) {
          return MapEntry(key, _maskSecret(item?.toString() ?? ''));
        }
        return MapEntry(key, _sanitizeValue(item));
      });
    }

    if (value is List) {
      return value.map(_sanitizeValue).toList();
    }

    return value;
  }

  bool _isSensitiveKey(String key) {
    return key.contains('password') ||
        key.contains('token') ||
        key == 'authorization' ||
        key == 'access_token';
  }

  String _sanitizeResponseBody(String body) {
    if (body.trim().isEmpty) return '<empty>';

    try {
      final decoded = jsonDecode(body);
      return _safeJsonEncode(_sanitizeValue(decoded));
    } catch (_) {
      return body
          .replaceAllMapped(
            RegExp(r'(Bearer\s+)[A-Za-z0-9._~+/=-]+', caseSensitive: false),
            (match) => '${match.group(1)}***',
          )
          .replaceAllMapped(
            RegExp(
              r'("(?:accessToken|AccessToken|token|Token)"\s*:\s*")[^"]+(")',
            ),
            (match) => '${match.group(1)}***${match.group(2)}',
          );
    }
  }

  String _safeJsonEncode(dynamic value) {
    try {
      return jsonEncode(value);
    } catch (_) {
      return value.toString();
    }
  }

  String _maskSecret(String value) {
    final trimmed = value.trim();
    if (trimmed.isEmpty) return '<empty>';
    if (trimmed.toLowerCase().startsWith('bearer ')) {
      return 'Bearer ***';
    }
    if (trimmed.length <= 4) return '***';
    return '${trimmed.substring(0, 2)}***${trimmed.substring(trimmed.length - 2)}';
  }
}

class LoginResult {
  const LoginResult({required this.accessToken, this.name});

  final String accessToken;
  final String? name;
}
