import 'package:flutter/material.dart';

import '../app.dart';

class AppCard extends StatelessWidget {
  const AppCard({required this.child, this.padding = 18, super.key});

  final Widget child;
  final double padding;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      padding: EdgeInsets.all(padding),
      decoration: BoxDecoration(
        color: Theme.of(context).brightness == Brightness.dark
            ? appSurface
            : Colors.white,
        borderRadius: BorderRadius.circular(22),
        border: Border.all(color: appPrimary.withValues(alpha: 0.14)),
      ),
      child: child,
    );
  }
}
