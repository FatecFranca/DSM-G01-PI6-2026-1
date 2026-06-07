import 'package:flutter/material.dart';

import '../app.dart';
import 'app_card.dart';

class MetricCard extends StatelessWidget {
  const MetricCard({
    required this.icon,
    required this.label,
    required this.value,
    this.detail,
    this.progress,
    super.key,
  });

  final IconData icon;
  final String label;
  final String value;
  final String? detail;
  final double? progress;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: appSecondary, size: 20),
              const SizedBox(width: 8),
              Expanded(
                child: Text(
                  label,
                  style: const TextStyle(color: appMuted, fontSize: 13),
                ),
              ),
            ],
          ),
          const SizedBox(height: 12),
          Text(
            value,
            style: Theme.of(context).textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.w800,
                  color: Theme.of(context).colorScheme.onSurface,
                ),
          ),
          if (detail != null) ...[
            const SizedBox(height: 6),
            Text(detail!, style: const TextStyle(color: appMuted)),
          ],
          if (progress != null) ...[
            const SizedBox(height: 12),
            ClipRRect(
              borderRadius: BorderRadius.circular(99),
              child: LinearProgressIndicator(
                value: progress!.clamp(0, 1),
                minHeight: 8,
                backgroundColor: appSurfaceAlt,
                valueColor: const AlwaysStoppedAnimation(appPrimary),
              ),
            ),
          ],
        ],
      ),
    );
  }
}
