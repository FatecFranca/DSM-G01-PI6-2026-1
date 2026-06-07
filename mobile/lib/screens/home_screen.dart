import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../app.dart';
import '../models/insights.dart';
import '../models/sleep_record.dart';
import '../routes/app_routes.dart';
import '../widgets/app_card.dart';
import '../widgets/bottom_nav.dart';
import '../widgets/custom_button.dart';
import '../widgets/metric_card.dart';

class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> {
  bool loading = true;
  String userName = 'Usuario';
  List<SleepRecord> history = const [];
  Insights? insights;

  @override
  void initState() {
    super.initState();
  }

  @override
  void didChangeDependencies() {
    super.didChangeDependencies();
    _load();
  }

  Future<void> _load() async {
    final deps = AppScope.of(context);
    final user = await deps.session.getCurrentUser();
    try {
      final results = await Future.wait([
        deps.api.getSleepHistory(),
        deps.api.getInsights(),
      ]);
      if (!mounted) return;
      setState(() {
        userName = user?.name.isNotEmpty == true ? user!.name : 'Usuario';
        history = results[0] as List<SleepRecord>;
        insights = results[1] as Insights;
        loading = false;
      });
    } catch (_) {
      if (!mounted) return;
      setState(() {
        userName = user?.name.isNotEmpty == true ? user!.name : 'Usuario';
        loading = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    final latest = history.isNotEmpty ? history.first : null;
    final averageSleep = insights?.averageSleep ?? _averageSleep(history);
    final averageScore = insights?.averageScore ?? _averageScore(history);

    return Scaffold(
      appBar: AppBar(
        title: Text('${_greeting()}, $userName'),
        actions: [
          IconButton(onPressed: _load, icon: const Icon(Icons.refresh_rounded)),
        ],
      ),
      bottomNavigationBar: const BottomNav(currentRoute: AppRoutes.home),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _load,
              child: ListView(
                padding: const EdgeInsets.all(18),
                children: [
                  Text(
                    DateFormat("EEEE, d 'de' MMMM 'de' y", 'pt_BR').format(DateTime.now()),
                    style: const TextStyle(color: appMuted),
                  ),
                  const SizedBox(height: 18),
                  AppCard(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Icon(Icons.nights_stay_rounded, color: appPrimary, size: 34),
                        const SizedBox(height: 12),
                        const Text('Sua Pontuacao de Sono', style: TextStyle(color: appMuted)),
                        const SizedBox(height: 8),
                        Text(
                          averageScore == null ? '--' : '${averageScore.round()}%',
                          style: const TextStyle(fontSize: 44, fontWeight: FontWeight.w900),
                        ),
                        const Text('Media calculada com dados reais do backend.'),
                      ],
                    ),
                  ),
                  const SizedBox(height: 14),
                  MetricCard(
                    icon: Icons.schedule_rounded,
                    label: 'Ultima noite',
                    value: latest == null ? 'Nao informado' : _formatMinutes(latest.sleepDuration),
                    detail: 'Ultimo registro recebido',
                  ),
                  const SizedBox(height: 14),
                  MetricCard(
                    icon: Icons.bedtime_rounded,
                    label: 'Sono medio',
                    value: averageSleep == null ? 'Nao informado' : '${averageSleep.toStringAsFixed(1)}h',
                    detail: 'Calculado pelos registros',
                  ),
                  const SizedBox(height: 14),
                  MetricCard(
                    icon: Icons.flag_rounded,
                    label: 'Meta atingida',
                    value: '${history.where((item) => item.sleepDuration >= 420).length}/${history.length}',
                    progress: history.isEmpty
                        ? 0
                        : history.where((item) => item.sleepDuration >= 420).length / history.length,
                  ),
                  const SizedBox(height: 18),
                  CustomButton(
                    label: 'Adicionar Dados de Sono',
                    icon: Icons.add_rounded,
                    onPressed: () => Navigator.of(context).pushReplacementNamed(AppRoutes.diary),
                  ),
                ],
              ),
            ),
    );
  }

  String _greeting() {
    final hour = DateTime.now().hour;
    if (hour >= 5 && hour < 12) return 'Bom dia';
    if (hour >= 12 && hour < 18) return 'Boa tarde';
    return 'Boa noite';
  }

  double? _averageSleep(List<SleepRecord> records) {
    if (records.isEmpty) return null;
    return records.map((record) => record.sleepDuration / 60).reduce((a, b) => a + b) / records.length;
  }

  double? _averageScore(List<SleepRecord> records) {
    final scores = records.where((record) => record.sleepScore != null).map((record) => record.sleepScore!);
    if (scores.isEmpty) return null;
    return scores.reduce((a, b) => a + b) / scores.length;
  }

  String _formatMinutes(int minutes) => '${minutes ~/ 60}h ${minutes % 60}m';
}
