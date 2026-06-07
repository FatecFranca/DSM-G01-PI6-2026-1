import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../app.dart';
import '../models/sleep_record.dart';
import '../routes/app_routes.dart';
import '../widgets/bottom_nav.dart';
import '../widgets/custom_button.dart';
import '../widgets/custom_input.dart';

class DiaryScreen extends StatefulWidget {
  const DiaryScreen({super.key});

  @override
  State<DiaryScreen> createState() => _DiaryScreenState();
}

class _DiaryScreenState extends State<DiaryScreen> {
  DateTime date = DateTime.now();
  TimeOfDay sleepTime = const TimeOfDay(hour: 22, minute: 30);
  TimeOfDay wakeTime = const TimeOfDay(hour: 6, minute: 30);
  double sleepQuality = 8;
  double stressLevel = 5;
  double mentalFatigue = 4;
  String physicalActivity = 'medium';
  final stepsController = TextEditingController(text: '7200');
  final heartRateController = TextEditingController(text: '72');
  final bloodPressureController = TextEditingController(text: '120');
  final screenTimeController = TextEditingController(text: '45');
  bool caffeine = false;
  bool alcohol = false;
  bool loading = false;
  String message = '';

  int get durationMinutes {
    final start = sleepTime.hour * 60 + sleepTime.minute;
    final end = wakeTime.hour * 60 + wakeTime.minute;
    final diff = end - start;
    return diff >= 0 ? diff : diff + 1440;
  }

  @override
  void dispose() {
    stepsController.dispose();
    heartRateController.dispose();
    bloodPressureController.dispose();
    screenTimeController.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    setState(() {
      loading = true;
      message = '';
    });

    final record = SleepRecord(
      date: DateFormat('yyyy-MM-dd').format(date),
      sleepDuration: durationMinutes,
      sleepQuality: sleepQuality.round(),
      stressLevel: stressLevel.round(),
      mentalFatigue: mentalFatigue.round(),
      physicalActivity: physicalActivity,
      steps: int.tryParse(stepsController.text) ?? 0,
      heartRate: int.tryParse(heartRateController.text) ?? 0,
      bloodPressure: int.tryParse(bloodPressureController.text) ?? 0,
      screenTime: int.tryParse(screenTimeController.text) ?? 0,
      caffeine: caffeine,
      alcohol: alcohol,
    );

    try {
      await AppScope.of(context).api.saveSleepRecord(record);
      if (!mounted) return;
      setState(() => message = 'Registro salvo e enviado ao backend.');
    } catch (error) {
      if (!mounted) return;
      setState(() => message = error.toString());
    } finally {
      if (mounted) setState(() => loading = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Diario')),
      bottomNavigationBar: const BottomNav(currentRoute: AppRoutes.diary),
      body: ListView(
        padding: const EdgeInsets.all(18),
        children: [
          Text('Duracao calculada: ${durationMinutes ~/ 60}h ${durationMinutes % 60}m',
              style: const TextStyle(fontSize: 24, fontWeight: FontWeight.w800)),
          const SizedBox(height: 16),
          ListTile(
            contentPadding: EdgeInsets.zero,
            title: const Text('Data'),
            subtitle: Text(DateFormat('dd/MM/yyyy').format(date)),
            trailing: const Icon(Icons.calendar_month_rounded),
            onTap: () async {
              final selected = await showDatePicker(
                context: context,
                initialDate: date,
                firstDate: DateTime(2020),
                lastDate: DateTime.now().add(const Duration(days: 1)),
              );
              if (selected != null) setState(() => date = selected);
            },
          ),
          _TimeTile(
            label: 'Horario que dormiu',
            value: sleepTime,
            onChanged: (value) => setState(() => sleepTime = value),
          ),
          _TimeTile(
            label: 'Horario que acordou',
            value: wakeTime,
            onChanged: (value) => setState(() => wakeTime = value),
          ),
          _SliderField(
            label: 'Qualidade do sono',
            value: sleepQuality,
            onChanged: (value) => setState(() => sleepQuality = value),
          ),
          _SliderField(
            label: 'Nivel de estresse',
            value: stressLevel,
            onChanged: (value) => setState(() => stressLevel = value),
          ),
          _SliderField(
            label: 'Fadiga mental',
            value: mentalFatigue,
            onChanged: (value) => setState(() => mentalFatigue = value),
          ),
          const SizedBox(height: 12),
          DropdownButtonFormField<String>(
            initialValue: physicalActivity,
            decoration: const InputDecoration(labelText: 'Atividade fisica'),
            items: const [
              DropdownMenuItem(value: 'low', child: Text('Baixa')),
              DropdownMenuItem(value: 'medium', child: Text('Media')),
              DropdownMenuItem(value: 'high', child: Text('Alta')),
            ],
            onChanged: (value) => setState(() => physicalActivity = value ?? physicalActivity),
          ),
          const SizedBox(height: 12),
          CustomInput(label: 'Passos', controller: stepsController, keyboardType: TextInputType.number),
          const SizedBox(height: 12),
          CustomInput(label: 'Frequencia cardiaca', controller: heartRateController, keyboardType: TextInputType.number),
          const SizedBox(height: 12),
          CustomInput(label: 'Pressao arterial', controller: bloodPressureController, keyboardType: TextInputType.number),
          const SizedBox(height: 12),
          CustomInput(label: 'Tempo de tela antes de dormir', controller: screenTimeController, keyboardType: TextInputType.number),
          SwitchListTile(
            value: caffeine,
            title: const Text('Cafeina'),
            onChanged: (value) => setState(() => caffeine = value),
          ),
          SwitchListTile(
            value: alcohol,
            title: const Text('Alcool'),
            onChanged: (value) => setState(() => alcohol = value),
          ),
          if (message.isNotEmpty) ...[
            const SizedBox(height: 8),
            Text(message, style: const TextStyle(color: appSecondary)),
          ],
          const SizedBox(height: 18),
          CustomButton(label: 'Salvar Registro', loading: loading, onPressed: _save),
        ],
      ),
    );
  }
}

class _TimeTile extends StatelessWidget {
  const _TimeTile({required this.label, required this.value, required this.onChanged});

  final String label;
  final TimeOfDay value;
  final ValueChanged<TimeOfDay> onChanged;

  @override
  Widget build(BuildContext context) {
    return ListTile(
      contentPadding: EdgeInsets.zero,
      title: Text(label),
      subtitle: Text(value.format(context)),
      trailing: const Icon(Icons.schedule_rounded),
      onTap: () async {
        final selected = await showTimePicker(context: context, initialTime: value);
        if (selected != null) onChanged(selected);
      },
    );
  }
}

class _SliderField extends StatelessWidget {
  const _SliderField({required this.label, required this.value, required this.onChanged});

  final String label;
  final double value;
  final ValueChanged<double> onChanged;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        const SizedBox(height: 14),
        Text('$label: ${value.round()}/10'),
        Slider(value: value, min: 1, max: 10, divisions: 9, onChanged: onChanged),
      ],
    );
  }
}
