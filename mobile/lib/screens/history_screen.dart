import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../app.dart';
import '../models/sleep_record.dart';
import '../routes/app_routes.dart';
import '../services/api_service.dart';
import '../widgets/app_card.dart';
import '../widgets/bottom_nav.dart';

class HistoryScreen extends StatefulWidget {
  const HistoryScreen({super.key});

  @override
  State<HistoryScreen> createState() => _HistoryScreenState();
}

class _HistoryScreenState extends State<HistoryScreen> {
  bool loading = true;
  bool _didLoad = false;
  String errorMessage = '';
  List<SleepRecord> records = const [];
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
      final data = await _api.getSleepHistory();
      if (!mounted) return;
      setState(() {
        records = data;
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
    return Scaffold(
      appBar: AppBar(title: const Text('Historico')),
      bottomNavigationBar: const BottomNav(currentRoute: AppRoutes.history),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : RefreshIndicator(
              onRefresh: _load,
              child: ListView(
                padding: const EdgeInsets.all(18),
                children: [
                  if (errorMessage.isNotEmpty)
                    AppCard(child: Text(errorMessage, style: const TextStyle(color: Color(0xFFFCA5A5)))),
                  if (records.isEmpty && errorMessage.isEmpty)
                    const AppCard(child: Text('Nenhum registro de sono encontrado.')),
                  for (final record in records) ...[
                    _RecordCard(record: record),
                    const SizedBox(height: 12),
                  ],
                ],
              ),
            ),
    );
  }
}

class _RecordCard extends StatelessWidget {
  const _RecordCard({required this.record});

  final SleepRecord record;

  @override
  Widget build(BuildContext context) {
    return InkWell(
      borderRadius: BorderRadius.circular(22),
      onTap: () => _showDetails(context),
      child: AppCard(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    _formatDate(record.date),
                    style: const TextStyle(fontSize: 18, fontWeight: FontWeight.w800),
                  ),
                ),
                Text('${record.sleepScore ?? '--'}%', style: const TextStyle(color: appPrimary)),
              ],
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 10,
              runSpacing: 10,
              children: [
                _Chip(label: 'Sono', value: _formatMinutes(record.sleepDuration)),
                _Chip(label: 'Qualidade', value: '${record.sleepQuality}/10'),
                _Chip(label: 'Estresse', value: '${record.stressLevel}/10'),
                _Chip(label: 'Fadiga', value: '${record.mentalFatigue}/10'),
              ],
            ),
            const SizedBox(height: 10),
            Text(record.disorder ?? 'Disturbio nao identificado', style: const TextStyle(color: appMuted)),
          ],
        ),
      ),
    );
  }

  void _showDetails(BuildContext context) {
    showModalBottomSheet(
      context: context,
      showDragHandle: true,
      backgroundColor: appSurface,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(28)),
      ),
      builder: (context) {
        return Padding(
          padding: const EdgeInsets.fromLTRB(20, 8, 20, 28),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text('Detalhes do registro', style: Theme.of(context).textTheme.titleLarge),
              const SizedBox(height: 16),
              _Detail('Data', _formatDate(record.date)),
              _Detail('Duracao', _formatMinutes(record.sleepDuration)),
              _Detail('Qualidade', '${record.sleepQuality}/10'),
              _Detail('Estresse', '${record.stressLevel}/10'),
              _Detail('Fadiga mental', '${record.mentalFatigue}/10'),
              _Detail('Atividade fisica', record.physicalActivity),
              _Detail('Passos', '${record.steps}'),
              _Detail('Frequencia cardiaca', '${record.heartRate} bpm'),
              _Detail('Pressao arterial', '${record.bloodPressure}'),
              _Detail('Tempo de tela', '${record.screenTime} min'),
              _Detail('Cafeina', record.caffeine ? 'Sim' : 'Nao'),
              _Detail('Alcool', record.alcohol ? 'Sim' : 'Nao'),
              _Detail('Score', record.sleepScore == null ? 'Nao informado' : '${record.sleepScore}%'),
              _Detail('Disturbio', record.disorder ?? 'Nao identificado'),
            ],
          ),
        );
      },
    );
  }

  String _formatMinutes(int minutes) => '${minutes ~/ 60}h ${minutes % 60}m';

  String _formatDate(String value) {
    final parsed = DateTime.tryParse(value);
    if (parsed == null) return value;
    return DateFormat('dd/MM/yyyy').format(parsed);
  }
}

class _Chip extends StatelessWidget {
  const _Chip({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Chip(label: Text('$label: $value'));
  }
}

class _Detail extends StatelessWidget {
  const _Detail(this.label, this.value);

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 5),
      child: Row(
        children: [
          Expanded(child: Text(label, style: const TextStyle(color: appMuted))),
          Flexible(child: Text(value, textAlign: TextAlign.end)),
        ],
      ),
    );
  }
}
