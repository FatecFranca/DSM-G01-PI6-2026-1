import 'package:flutter/material.dart';

import '../app.dart';
import '../routes/app_routes.dart';
import '../widgets/app_card.dart';
import '../widgets/custom_button.dart';

class OnboardingScreen extends StatelessWidget {
  const OnboardingScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(24),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              const Text(
                'Santuario do Sono',
                style: TextStyle(fontWeight: FontWeight.w800, fontSize: 18),
              ),
              const Spacer(),
              Container(
                height: 220,
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(32),
                  gradient: const LinearGradient(
                    colors: [Color(0xFF7C3AED), Color(0xFF0EA5E9)],
                    begin: Alignment.topLeft,
                    end: Alignment.bottomRight,
                  ),
                ),
                child: const Center(
                  child: Icon(Icons.nights_stay_rounded, size: 92, color: appText),
                ),
              ),
              const SizedBox(height: 28),
              const Text(
                'Seu descanso perfeito, guiado por IA.',
                style: TextStyle(fontSize: 34, height: 1.05, fontWeight: FontWeight.w900),
              ),
              const SizedBox(height: 14),
              const Text(
                'Entenda seus padroes de sono e melhore seu descanso com insights personalizados.',
                style: TextStyle(color: appMuted, fontSize: 16, height: 1.45),
              ),
              const SizedBox(height: 24),
              const AppCard(
                child: Row(
                  children: [
                    Icon(Icons.auto_awesome_rounded, color: appSecondary),
                    SizedBox(width: 12),
                    Expanded(
                      child: Text(
                        'Registre suas noites, acompanhe sua evolucao e receba recomendacoes simples.',
                      ),
                    ),
                  ],
                ),
              ),
              const Spacer(),
              CustomButton(
                label: 'Comecar',
                icon: Icons.arrow_forward_rounded,
                onPressed: () {
                  debugPrint("Navegando para login");
                  Navigator.of(context).pushNamed(AppRoutes.login);
                },
              ),
              const SizedBox(height: 12),
              CustomButton(
                label: 'Criar conta',
                secondary: true,
                onPressed: () => Navigator.of(context).pushNamed(AppRoutes.register),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
