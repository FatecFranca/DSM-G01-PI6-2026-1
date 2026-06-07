import 'package:flutter/material.dart';

import '../app.dart';
import '../models/insights.dart';
import '../models/sleep_record.dart';
import '../routes/app_routes.dart';
import '../services/api_service.dart';
import '../widgets/app_card.dart';
import '../widgets/bottom_nav.dart';
import '../widgets/metric_card.dart';

class InsightsScreen extends StatefulWidget {
  const InsightsScreen({super.key});

  @override
  State<InsightsScreen> createState() => _InsightsScreenState();
}

class _InsightsScreenState extends State<InsightsScreen> {
  bool loading = true;
  bool _didLoad = false;
  String errorMessage = '';
  Insights? insights;
  List<SleepRecord> history = const [];
  late ApiService _api;

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    if (_didLoad) return;

    _didLoad = true;
    _api = AppScope.of(context).api;
    _load();
  }

  Future<void> _load() async {
    setState(() {
      loading = true;
      errorMessage = '';
    });
    try {
      final results = await Future.wait([
        _api.getInsights(),
        _api.getSleepHistory(),
      ]);
      if (!mounted) return;
      setState(() {
        insights = results[0] as Insights;
        history = results[1] as List<SleepRecord>;
        loading = false;
      });
    } catch (error) {
      if (!mounted) return;
      setState(() {
        errorMessage = error.toString();
        loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final data = insights;
    final goalCount = history.where((record) => record.sleepDuration >= 420).length;
    final goalProgress = history.isEmpty ? 0.0 : goalCount / history.length;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Insights'),
        actions: [IconButton(onPressed: _load, icon: const Icon(Icons.refresh_rounded))],
      ),
      bottomNavigationBar: const BottomNav(currentRoute: AppRoutes.insights),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _load,
              child: ListView(
                padding: const EdgeInsets.all(18),
                children: [
                  if (errorMessage.isNotEmpty)
                    AppCard(child: Text(errorMessage, style: const TextStyle(color: Color(0xFFFCA5A5)))),
                  MetricCard(
                    icon: Icons.bedtime_rounded,
                    label: 'Sono medio',
                    value: data == null ? 'Nao informado' : '${data.averageSleep.toStringAsFixed(1)}h',
                    detail: 'Media retornada pelos insights',
                  ),
                  const SizedBox(height: 12),
                  MetricCard(
                    icon: Icons.bolt_rounded,
                    label: 'Score medio',
                    value: data == null ? 'Nao informado' : '${data.averageScore.round()}%',
                    progress: data == null ? 0 : data.averageScore / 100,
                  ),
                  const SizedBox(height: 12),
                  MetricCard(
                    icon: Icons.flag_rounded,
                    label: 'Noites na meta',
                    value: '$goalCount/${history.length}',
                    progress: goalProgress,
                  ),
                  const SizedBox(height: 18),
                  _ListSection(
                    title: 'Padroes identificados',
                    icon: Icons.psychology_rounded,
                    items: data?.patterns ?? const [],
                    empty: 'Nenhum padrao disponivel no momento.',
                  ),
                  const SizedBox(height: 14),
                  _ListSection(
                    title: 'Recomendacoes',
                    icon: Icons.tips_and_updates_rounded,
                    items: data?.recommendations ?? const [],
                    empty: 'Sem recomendacoes disponiveis no momento.',
                  ),
                ],
              ),
            ),
    );
  }
}

class _ListSection extends StatelessWidget {
  const _ListSection({
    required this.title,
    required this.icon,
    required this.items,
    required this.empty,
  });

  final String title;
  final IconData icon;
  final List<String> items;
  final String empty;

  @override
  Widget build(BuildContext context) {
    return AppCard(
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Icon(icon, color: appSecondary),
              const SizedBox(width: 10),
              Text(title, style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w800)),
            ],
          ),
          const SizedBox(height: 12),
          if (items.isEmpty)
            Text(empty, style: const TextStyle(color: appMuted))
          else
            for (final item in items)
              Padding(
                padding: const EdgeInsets.symmetric(vertical: 6),
                child: Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Icon(Icons.check_circle_rounded, color: appPrimary, size: 20),
                    const SizedBox(width: 8),
                    Expanded(child: Text(item)),
                  ],
                ),
              ),
        ],
      ),
    );
  }
}
