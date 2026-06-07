import 'package:flutter/material.dart';
import 'package:intl/intl.dart';

import '../app.dart';
import '../models/user_profile.dart';
import '../routes/app_routes.dart';
import '../widgets/app_card.dart';
import '../widgets/bottom_nav.dart';
import '../widgets/custom_button.dart';
import '../widgets/custom_input.dart';
import '../widgets/metric_card.dart';

class ProfileScreen extends StatefulWidget {
  const ProfileScreen({super.key});

  @override
  State<ProfileScreen> createState() => _ProfileScreenState();
}

class _ProfileScreenState extends State<ProfileScreen> {
  bool loading = true;
  String errorMessage = '';
  UserProfile? profile;

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
    setState(() {
      loading = true;
      errorMessage = '';
    });
    final api = AppScope.of(context).api;
    final session = AppScope.of(context).session;
    try {
      final apiProfile = await api.getUserProfile();
      await session.saveCurrentUser(apiProfile);
      if (!mounted) return;
      setState(() {
        profile = apiProfile;
        loading = false;
      });
    } catch (error) {
      if (!mounted) return;
      debugPrint("Sessão inválida, redirecionando para login");
      await session.logout();
      if (!mounted) return;
      Navigator.of(context).pushNamedAndRemoveUntil(AppRoutes.login, (_) => false);
    }
  }

  Future<void> _logout() async {
    final session = AppScope.of(context).session;
    final navigator = Navigator.of(context);
    await session.logout();
    if (!mounted) return;
    navigator.pushNamedAndRemoveUntil(AppRoutes.login, (_) => false);
  }

  Future<void> _editProfile() async {
    final current = profile;
    if (current == null) return;
    final session = AppScope.of(context).session;
    final updated = await showModalBottomSheet<UserProfile>(
      context: context,
      isScrollControlled: true,
      showDragHandle: true,
      backgroundColor: appSurface,
      builder: (context) => _ProfileEditor(profile: current),
    );
    if (!mounted) return;
    if (updated == null) return;
    await session.saveCurrentUser(updated);
    if (!mounted) return;
    setState(() => profile = updated);
  }

  @override
  Widget build(BuildContext context) {
    final data = profile;
    final bmi = data == null ? null : _calculateBmi(data.weight, data.height);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Perfil'),
        actions: [
          IconButton(onPressed: _editProfile, icon: const Icon(Icons.settings_rounded)),
        ],
      ),
      bottomNavigationBar: const BottomNav(currentRoute: AppRoutes.profile),
      body: loading
          ? const Center(child: CircularProgressIndicator())
          : ListView(
              padding: const EdgeInsets.all(18),
              children: [
                if (errorMessage.isNotEmpty) ...[
                  AppCard(child: Text(errorMessage, style: const TextStyle(color: appSecondary))),
                  const SizedBox(height: 12),
                ],
                AppCard(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Icon(Icons.person_rounded, color: appPrimary, size: 40),
                      const SizedBox(height: 12),
                      Text(
                        data?.name.isNotEmpty == true ? data!.name : 'Usuario',
                        style: const TextStyle(fontSize: 30, fontWeight: FontWeight.w900),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        data == null ? 'Dados nao informados' : 'Meta de sono: ${data.sleepGoal.toStringAsFixed(0)}h',
                        style: const TextStyle(color: appMuted),
                      ),
                    ],
                  ),
                ),
                const SizedBox(height: 12),
                MetricCard(
                  icon: Icons.cake_rounded,
                  label: 'Data de nascimento',
                  value: _formatBirthDate(data?.birthDate ?? ''),
                ),
                const SizedBox(height: 12),
                MetricCard(
                  icon: Icons.monitor_weight_rounded,
                  label: 'Peso',
                  value: data == null || data.weight <= 0 ? 'Nao informado' : '${data.weight.toStringAsFixed(1)} kg',
                ),
                const SizedBox(height: 12),
                MetricCard(
                  icon: Icons.height_rounded,
                  label: 'Altura',
                  value: data == null || data.height <= 0 ? 'Nao informado' : '${data.height.toStringAsFixed(0)} cm',
                ),
                const SizedBox(height: 12),
                MetricCard(
                  icon: Icons.analytics_rounded,
                  label: 'IMC',
                  value: bmi == null ? 'Nao informado' : bmi.toStringAsFixed(2),
                  detail: bmi == null ? 'Complete peso e altura' : _bmiClassification(bmi),
                ),
                const SizedBox(height: 12),
                MetricCard(
                  icon: Icons.wc_rounded,
                  label: 'Genero',
                  value: data?.gender.isNotEmpty == true ? data!.gender : 'Nao informado',
                ),
                const SizedBox(height: 18),
                CustomButton(label: 'Sair', icon: Icons.logout_rounded, secondary: true, onPressed: _logout),
              ],
            ),
    );
  }

  double? _calculateBmi(double weight, double height) {
    if (weight <= 0 || height <= 0) return null;
    final meters = height > 3 ? height / 100 : height;
    return weight / (meters * meters);
  }

  String _bmiClassification(double bmi) {
    if (bmi < 18.5) return 'Abaixo do peso';
    if (bmi < 25) return 'Peso normal';
    if (bmi < 30) return 'Sobrepeso';
    if (bmi < 35) return 'Obesidade grau I';
    if (bmi < 40) return 'Obesidade grau II';
    return 'Obesidade grau III';
  }

  String _formatBirthDate(String value) {
    final parsed = DateTime.tryParse(value);
    if (parsed == null) return value.isEmpty ? 'Nao informado' : value;
    return DateFormat('dd/MM/yyyy').format(parsed);
  }
}

class _ProfileEditor extends StatefulWidget {
  const _ProfileEditor({required this.profile});

  final UserProfile profile;

  @override
  State<_ProfileEditor> createState() => _ProfileEditorState();
}

class _ProfileEditorState extends State<_ProfileEditor> {
  late final TextEditingController nameController;
  late final TextEditingController birthDateController;
  late final TextEditingController weightController;
  late final TextEditingController heightController;
  late String gender;
  late double sleepGoal;
  late bool darkTheme;

  @override
  void initState() {
    super.initState();
    nameController = TextEditingController(text: widget.profile.name);
    birthDateController = TextEditingController(text: widget.profile.birthDate);
    weightController = TextEditingController(text: widget.profile.weight == 0 ? '' : widget.profile.weight.toString());
    heightController = TextEditingController(text: widget.profile.height == 0 ? '' : widget.profile.height.toString());
    gender = widget.profile.gender.isEmpty ? 'female' : widget.profile.gender;
    sleepGoal = widget.profile.sleepGoal == 0 ? 8 : widget.profile.sleepGoal;
    darkTheme = true;
  }

  @override
  void dispose() {
    nameController.dispose();
    birthDateController.dispose();
    weightController.dispose();
    heightController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final bottomInset = MediaQuery.of(context).viewInsets.bottom;
    return Padding(
      padding: EdgeInsets.fromLTRB(20, 8, 20, 20 + bottomInset),
      child: SingleChildScrollView(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Configuracoes', style: Theme.of(context).textTheme.titleLarge),
            const SizedBox(height: 16),
            CustomInput(label: 'Nome', controller: nameController),
            const SizedBox(height: 12),
            CustomInput(label: 'Data de nascimento', controller: birthDateController),
            const SizedBox(height: 12),
            DropdownButtonFormField<String>(
              initialValue: gender,
              decoration: const InputDecoration(labelText: 'Genero'),
              items: const [
                DropdownMenuItem(value: 'female', child: Text('Feminino')),
                DropdownMenuItem(value: 'male', child: Text('Masculino')),
              ],
              onChanged: (value) => setState(() => gender = value ?? gender),
            ),
            const SizedBox(height: 12),
            CustomInput(label: 'Peso', controller: weightController, keyboardType: TextInputType.number),
            const SizedBox(height: 12),
            CustomInput(label: 'Altura', controller: heightController, keyboardType: TextInputType.number),
            SwitchListTile(
              value: darkTheme,
              title: const Text('Tema escuro'),
              onChanged: (value) async {
                setState(() => darkTheme = value);
                await AppScope.of(context).onThemeChanged(value ? ThemeMode.dark : ThemeMode.light);
              },
            ),
            Row(
              children: [
                const Expanded(child: Text('Meta de sono')),
                Text('${sleepGoal.toStringAsFixed(0)}h'),
              ],
            ),
            Slider(
              value: sleepGoal,
              min: 4,
              max: 12,
              divisions: 8,
              onChanged: (value) => setState(() => sleepGoal = value),
            ),
            const SizedBox(height: 12),
            CustomButton(
              label: 'Salvar',
              onPressed: () {
                Navigator.of(context).pop(
                  UserProfile(
                    name: nameController.text.trim(),
                    birthDate: birthDateController.text.trim(),
                    weight: double.tryParse(weightController.text) ?? 0,
                    height: double.tryParse(heightController.text) ?? 0,
                    gender: gender,
                    sleepGoal: sleepGoal,
                    occupation: widget.profile.occupation,
                  ),
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}
