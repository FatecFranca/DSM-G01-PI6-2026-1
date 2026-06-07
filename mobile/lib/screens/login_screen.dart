import 'package:flutter/material.dart';

import '../app.dart';
import '../models/user_profile.dart';
import '../routes/app_routes.dart';
import '../services/api_service.dart';
import '../widgets/custom_button.dart';
import '../widgets/custom_input.dart';

class LoginScreen extends StatefulWidget {
  const LoginScreen({super.key});

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final emailController = TextEditingController();
  final passwordController = TextEditingController();
  String errorMessage = '';
  bool loading = false;

  @override
  void dispose() {
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  Future<void> _login() async {
    if (loading) {
      debugPrint('[LoginScreen] Login ignorado: requisicao ja em andamento.');
      return;
    }

    if (emailController.text.trim().isEmpty || passwordController.text.isEmpty) {
      debugPrint('[LoginScreen] Login bloqueado: email ou senha vazios.');
      setState(() => errorMessage = 'Informe e-mail e senha para continuar.');
      return;
    }

    setState(() {
      loading = true;
      errorMessage = '';
    });

    final dependencies = AppScope.of(context);
    final email = emailController.text.trim();
    debugPrint("Tentando login no backend");
    debugPrint('[LoginScreen] Login iniciado. email=$email');

    try {
      final result = await dependencies.api.login(email, passwordController.text);
      debugPrint('[LoginScreen] Backend aceitou credenciais. Solicitando perfil.');

      final profile = await dependencies.api.getUserProfile(accessToken: result.accessToken);
      if (profile.name.trim().isEmpty) {
        throw const ApiException('Resposta de perfil incompleta.');
      }

      debugPrint('[LoginScreen] Perfil recebido. Salvando sessao.');
      await dependencies.session.saveAuthenticatedSession(
        accessToken: result.accessToken,
        profile: profile,
      );

      if (!mounted) return;
      debugPrint("Login confirmado pelo backend");
      debugPrint('[LoginScreen] Login permitido: navegando para home.');
      Navigator.of(context).pushReplacementNamed(AppRoutes.home);
    } catch (error) {
      if (!mounted) return;
      final message = error.toString();
      if (message == backendUnavailableMessage) {
        debugPrint("Backend indisponível. Login bloqueado.");
      }
      debugPrint('[LoginScreen] Login bloqueado: $error');
      setState(() {
        errorMessage = message;
      });
    } finally {
      if (mounted) {
        setState(() => loading = false);
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: SingleChildScrollView(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              IconButton(
                onPressed: () => Navigator.of(context).pushReplacementNamed(AppRoutes.onboarding),
                icon: const Icon(Icons.arrow_back_rounded),
              ),
              const SizedBox(height: 48),
              const Text('Entrar', style: TextStyle(fontSize: 34, fontWeight: FontWeight.w900)),
              const SizedBox(height: 8),
              const Text(
                'Acesse sua conta para continuar sua jornada de sono.',
                style: TextStyle(color: appMuted),
              ),
              const SizedBox(height: 28),
              CustomInput(
                label: 'E-mail',
                controller: emailController,
                keyboardType: TextInputType.emailAddress,
              ),
              const SizedBox(height: 14),
              CustomInput(
                label: 'Senha',
                controller: passwordController,
                obscureText: true,
              ),
              if (errorMessage.isNotEmpty) ...[
                const SizedBox(height: 12),
                Text(errorMessage, style: const TextStyle(color: Color(0xFFFCA5A5))),
              ],
              const SizedBox(height: 24),
              CustomButton(label: 'Entrar', loading: loading, onPressed: _login),
              const SizedBox(height: 20),
              Center(
                child: TextButton(
                  onPressed: () => Navigator.of(context).pushReplacementNamed(AppRoutes.register),
                  child: const Text('Ainda nao tem uma conta? Criar conta'),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
