import 'package:flutter_test/flutter_test.dart';
import 'package:sleep_sanctuary_mobile/app.dart';

void main() {
  testWidgets('renders onboarding screen', (tester) async {
    await tester.pumpWidget(const SleepSanctuaryApp());
    await tester.pumpAndSettle();

    expect(find.text('Santuario do Sono'), findsOneWidget);
    expect(find.text('Comecar'), findsOneWidget);
  });
}
