class AppRoutes {
  static const onboarding = '/';
  static const login = '/login';
  static const register = '/register';
  static const home = '/home';
  static const diary = '/diary';
  static const insights = '/insights';
  static const history = '/history';
  static const profile = '/profile';

  static const protectedRoutes = {
    home,
    diary,
    insights,
    history,
    profile,
  };
}
