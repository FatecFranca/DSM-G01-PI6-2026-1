import 'package:flutter/material.dart';

import 'routes/app_routes.dart';
import 'screens/diary_screen.dart';
import 'screens/history_screen.dart';
import 'screens/home_screen.dart';
import 'screens/insights_screen.dart';
import 'screens/login_screen.dart';
import 'screens/onboarding_screen.dart';
import 'screens/profile_screen.dart';
import 'screens/register_screen.dart';
import 'services/api_service.dart';
import 'services/session_service.dart';

const appBackground = Color(0xFF0E1021);
const appSurface = Color(0xFF1A1730);
const appSurfaceAlt = Color(0xFF251D46);
const appPrimary = Color(0xFFA78BFA);
const appSecondary = Color(0xFF7DD3FC);
const appText = Color(0xFFF8FAFC);
const appMuted = Color(0xFFB8B5D6);

class SleepSanctuaryApp extends StatefulWidget {
  const SleepSanctuaryApp({super.key});

  @override
  State<SleepSanctuaryApp> createState() => _SleepSanctuaryAppState();
}

class _SleepSanctuaryAppState extends State<SleepSanctuaryApp> {
  final SessionService session = SessionService();
  late final ApiService api = ApiService(session: session);
  ThemeMode themeMode = ThemeMode.dark;

  @override
  void initState() {
    super.initState();
    _loadTheme();
  }

  Future<void> _loadTheme() async {
    final stored = await session.getThemeModeName();
    setState(() {
      themeMode = stored == 'light' ? ThemeMode.light : ThemeMode.dark;
    });
  }

  Future<void> setThemeMode(ThemeMode mode) async {
    setState(() => themeMode = mode);
    await session.saveThemeModeName(mode == ThemeMode.light ? 'light' : 'dark');
  }

  @override
  Widget build(BuildContext context) {
    final dependencies = AppDependencies(
      api: api,
      session: session,
      themeMode: themeMode,
      onThemeChanged: setThemeMode,
    );

    return AppScope(
      dependencies: dependencies,
      child: MaterialApp(
        title: 'Santuario do Sono',
        debugShowCheckedModeBanner: false,
        themeMode: themeMode,
        theme: _buildTheme(Brightness.light),
        darkTheme: _buildTheme(Brightness.dark),
        initialRoute: AppRoutes.onboarding,
        onGenerateRoute: (settings) {
          return MaterialPageRoute(
            builder: (context) => _GuardedRoute(
              routeName: settings.name ?? AppRoutes.onboarding,
              child: _screenFor(settings.name),
            ),
          );
        },
      ),
    );
  }

  Widget _screenFor(String? route) {
    switch (route) {
      case AppRoutes.login:
        return const LoginScreen();
      case AppRoutes.register:
        return const RegisterScreen();
      case AppRoutes.home:
        return const HomeScreen();
      case AppRoutes.diary:
        return const DiaryScreen();
      case AppRoutes.insights:
        return const InsightsScreen();
      case AppRoutes.history:
        return const HistoryScreen();
      case AppRoutes.profile:
        return const ProfileScreen();
      case AppRoutes.onboarding:
      default:
        return const OnboardingScreen();
    }
  }

  ThemeData _buildTheme(Brightness brightness) {
    final isDark = brightness == Brightness.dark;
    final scheme = ColorScheme.fromSeed(
      seedColor: appPrimary,
      brightness: brightness,
      primary: appPrimary,
      secondary: appSecondary,
      surface: isDark ? appSurface : Colors.white,
    );

    return ThemeData(
      useMaterial3: true,
      brightness: brightness,
      colorScheme: scheme,
      scaffoldBackgroundColor: isDark ? appBackground : const Color(0xFFF4F1FF),
      fontFamily: 'Roboto',
      appBarTheme: AppBarTheme(
        backgroundColor: Colors.transparent,
        foregroundColor: isDark ? appText : const Color(0xFF221A3F),
        elevation: 0,
        centerTitle: false,
      ),
      cardTheme: CardThemeData(
        color: isDark ? appSurface : Colors.white,
        elevation: 0,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(22)),
      ),
      inputDecorationTheme: InputDecorationTheme(
        filled: true,
        fillColor: isDark ? appSurfaceAlt : const Color(0xFFEDE7FF),
        border: OutlineInputBorder(
          borderRadius: BorderRadius.circular(18),
          borderSide: BorderSide.none,
        ),
        labelStyle: TextStyle(color: isDark ? appMuted : const Color(0xFF5F5478)),
      ),
    );
  }
}

class AppDependencies {
  const AppDependencies({
    required this.api,
    required this.session,
    required this.themeMode,
    required this.onThemeChanged,
  });

  final ApiService api;
  final SessionService session;
  final ThemeMode themeMode;
  final Future<void> Function(ThemeMode mode) onThemeChanged;
}

class AppScope extends InheritedWidget {
  const AppScope({
    required this.dependencies,
    required super.child,
    super.key,
  });

  final AppDependencies dependencies;

  static AppDependencies of(BuildContext context) {
    final scope = context.dependOnInheritedWidgetOfExactType<AppScope>();
    assert(scope != null, 'AppScope nao encontrado.');
    return scope!.dependencies;
  }

  @override
  bool updateShouldNotify(AppScope oldWidget) {
    return dependencies.themeMode != oldWidget.dependencies.themeMode;
  }
}

class _GuardedRoute extends StatefulWidget {
  const _GuardedRoute({required this.routeName, required this.child});

  final String routeName;
  final Widget child;

  @override
  State<_GuardedRoute> createState() => _GuardedRouteState();
}

class _GuardedRouteState extends State<_GuardedRoute> {
  bool? authenticated;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _checkSession();
  }

  Future<void> _checkSession() async {
    final dependencies = AppScope.of(context);
    final isProtected = AppRoutes.protectedRoutes.contains(widget.routeName);
    final isAuthRoute =
        widget.routeName == AppRoutes.login || widget.routeName == AppRoutes.register;
    var isAuthenticated = await dependencies.session.isAuthenticated();

    if (isAuthenticated && (isProtected || isAuthRoute)) {
      try {
        final profile = await dependencies.api.getUserProfile();
        await dependencies.session.saveCurrentUser(profile);
      } catch (_) {
        debugPrint("Sessão inválida, redirecionando para login");
        await dependencies.session.logout();
        isAuthenticated = false;
      }
    }

    if (!mounted) return;
    setState(() => authenticated = isAuthenticated);
    if (!isAuthenticated && isProtected) {
      debugPrint("Sessão inválida, redirecionando para login");
      Navigator.of(context).pushReplacementNamed(AppRoutes.login);
    }
    if (isAuthenticated && isAuthRoute) {
      Navigator.of(context).pushReplacementNamed(AppRoutes.home);
    }
  }

  @override
  Widget build(BuildContext context) {
    if (authenticated == null &&
        AppRoutes.protectedRoutes.contains(widget.routeName)) {
      return const Scaffold(body: Center(child: CircularProgressIndicator()));
    }
    if (authenticated == false &&
        AppRoutes.protectedRoutes.contains(widget.routeName)) {
      return const LoginScreen();
    }
    return widget.child;
  }
}
