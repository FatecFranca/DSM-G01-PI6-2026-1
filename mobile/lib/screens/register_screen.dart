import 'package:flutter/material.dart';

import '../app.dart';
import '../models/user_profile.dart';
import '../routes/app_routes.dart';
import '../widgets/custom_button.dart';
import '../widgets/custom_input.dart';

class RegisterScreen extends StatefulWidget {
  const RegisterScreen({super.key});

  @override
  State<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends State<RegisterScreen> {
  final nameController = TextEditingController();
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  final birthDateController = TextEditingController();
  final occupationController = TextEditingController();
  final weightController = TextEditingController();
  final heightController = TextEditingController();
  String gender = 'female';
  double sleepGoal = 8;
  String errorMessage = '';
  bool loading = false;

  @override
  void dispose() {
    nameController.dispose();
    emailController.dispose();
    passwordController.dispose();
    birthDateController.dispose();
    occupationController.dispose();
    weightController.dispose();
    heightController.dispose();
    super.dispose();
  }

  Future<void> _register() async {
    if (loading) return;

    if (nameController.text.trim().isEmpty ||
        emailController.text.trim().isEmpty ||
        passwordController.text.isEmpty ||
        birthDateController.text.trim().isEmpty) {
      setState(() => errorMessage = 'Preencha os campos obrigatorios.');
      return;
    }

    setState(() {
      loading = true;
      errorMessage = '';
    });

    final birthDate = _normalizeBirthDate(birthDateController.text);
    final weight = double.tryParse(weightController.text) ?? 0;
    final height = double.tryParse(heightController.text) ?? 0;
    final profile = UserProfile(
      name: nameController.text.trim(),
      birthDate: birthDate,
      weight: weight,
      height: height,
      gender: _genderToApi(gender),
      sleepGoal: sleepGoal,
      occupation: occupationController.text.trim(),
    );

    final dependencies = AppScope.of(context);
    try {
      final result = await dependencies.api.registerUser({
        'name': profile.name,
        'email': emailController.text.trim(),
        'password': passwordController.text,
        'birthDate': '${profile.birthDate}T00:00:00',
        'gender': profile.gender,
        'heightCm': height,
        'weightKg': weight,
        'occupation': profile.occupation,
        'sleepDisorder': 0,
      });

      await dependencies.session.saveAuthenticatedSession(
        accessToken: result.accessToken,
        profile: result.name?.isNotEmpty == true
            ? UserProfile(
                name: result.name!,
                birthDate: profile.birthDate,
                weight: profile.weight,
                height: profile.height,
                gender: profile.gender,
                sleepGoal: profile.sleepGoal,
                occupation: profile.occupation,
              )
            : profile,
      );

      if (!mounted) return;
      Navigator.of(context).pushReplacementNamed(AppRoutes.home);
    } catch (error) {
      if (!mounted) return;
      setState(() => errorMessage = error.toString());
    } finally {
      if (mounted) {
        setState(() => loading = false);
      }
    }
  }

  String _normalizeBirthDate(String value) {
    final digits = value.replaceAll(RegExp(r'\D'), '');
    if (digits.length == 8) {
      return '${digits.substring(4)}-${digits.substring(2, 4)}-${digits.substring(0, 2)}';
    }
    return value.trim();
  }

  String _genderToApi(String value) {
    switch (value) {
      case 'female':
        return 'F';
      case 'male':
        return 'M';
      default:
        return value;
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Criar conta')),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(24),
        child: Column(
          children: [
            CustomInput(label: 'Nome completo', controller: nameController),
            const SizedBox(height: 12),
            CustomInput(
              label: 'E-mail',
              controller: emailController,
              keyboardType: TextInputType.emailAddress,
            ),
            const SizedBox(height: 12),
            CustomInput(label: 'Senha', controller: passwordController, obscureText: true),
            const SizedBox(height: 12),
            CustomInput(
              label: 'Data de nascimento',
              controller: birthDateController,
              keyboardType: TextInputType.datetime,
            ),
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
            CustomInput(
              label: 'Ocupação',
              hintText: 'Ex: estudante, professor, desenvolvedor, enfermeiro',
              controller: occupationController,
            ),
            const SizedBox(height: 12),
            CustomInput(
              label: 'Peso (kg)',
              controller: weightController,
              keyboardType: TextInputType.number,
            ),
            const SizedBox(height: 12),
            CustomInput(
              label: 'Altura (cm)',
              controller: heightController,
              keyboardType: TextInputType.number,
            ),
            const SizedBox(height: 18),
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
            if (errorMessage.isNotEmpty)
              Text(errorMessage, style: const TextStyle(color: Color(0xFFFCA5A5))),
            const SizedBox(height: 18),
            CustomButton(label: 'Criar conta', loading: loading, onPressed: _register),
            TextButton(
              onPressed: () => Navigator.of(context).pushReplacementNamed(AppRoutes.login),
              child: const Text('Ja tem uma conta? Fazer login'),
            ),
          ],
        ),
      ),
    );
  }
}
