import 'package:flutter/material.dart';

import '../routes/app_routes.dart';

class BottomNav extends StatelessWidget {
  const BottomNav({required this.currentRoute, super.key});

  final String currentRoute;

  @override
  Widget build(BuildContext context) {
    const items = [
      _NavItem(AppRoutes.home, 'Home', Icons.home_rounded),
      _NavItem(AppRoutes.diary, 'Diario', Icons.edit_note_rounded),
      _NavItem(AppRoutes.insights, 'Insights', Icons.insights_rounded),
      _NavItem(AppRoutes.history, 'Historico', Icons.history_rounded),
      _NavItem(AppRoutes.profile, 'Perfil', Icons.person_rounded),
    ];

    return NavigationBar(
      selectedIndex: items.indexWhere((item) => item.route == currentRoute),
      onDestinationSelected: (index) {
        final route = items[index].route;
        if (route != currentRoute) {
          Navigator.of(context).pushReplacementNamed(route);
        }
      },
      destinations: [
        for (final item in items)
          NavigationDestination(icon: Icon(item.icon), label: item.label),
      ],
    );
  }
}

class _NavItem {
  const _NavItem(this.route, this.label, this.icon);

  final String route;
  final String label;
  final IconData icon;
}
