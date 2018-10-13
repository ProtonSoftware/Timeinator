# Timeinator

https://docs.google.com/document/d/1VpCnl0fz3ROxbVLxfZ4WzO-_ov4YlYcmyr8OJUiuIrw/edit <br><br>

Zadania:<br>
  &nbsp;-Stałe: - funkcja przyszłościowa<br>
    &nbsp;&nbsp;&nbsp;-w określonym z góry czasie (np. kurs śpiewania od 18:00 do 20:00)<br>
    &nbsp;&nbsp;&nbsp;-apka w tym czasie nie proponuje innych zadań bo wie że jesteś zajęty<br>
    &nbsp;&nbsp;&nbsp;-może być pobierany z jakiegoś źródła (skomplikowane więc to później)<br>
    &nbsp;&nbsp;&nbsp;-sen<br>
  &nbsp;-Bez określonego czasu (w dowolnej chwili wolnego czasu):<br>
    &nbsp;&nbsp;&nbsp;-obiad<br>
    &nbsp;&nbsp;&nbsp;-o określonym priorytecie (skala)<br>
    &nbsp;&nbsp;&nbsp;-mogą być cykliczne lub jednorazowe<br>
    &nbsp;&nbsp;&nbsp;-proponuje najodpowiedniejsze zadanie w aktualnej przerwie między zadaniami stałymi (wolny czas)<br>
    &nbsp;&nbsp;&nbsp;-Czas na wykonanie:<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-czas przydzielany priorytetowo (algorytm czasu)<br>
      &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;-czas ustawiony na stałe np. 15 minut<br><br>

Zakończenie zadania:<br>
  &nbsp;&nbsp;-powiadomienie gdy skończy się czas<br>
  &nbsp;&nbsp;-możliwość przedłużenia czasu zadania<br>
  &nbsp;&nbsp;-konieczne jest zatwierdzenie zakończenia zadania<br><br>
   
Algorytm czasu:<br>
  &nbsp;&nbsp;-{pozostały czas wolny} * {priorytet zadania} / {suma priorytetów zadań}<br>
  &nbsp;&nbsp;-dla takiego algorytmu konieczne jest podzielenie zadań na te ze stałym czasem i bez<br>
  &nbsp;&nbsp;-im więcej rzeczy jednego dnia zrobić np. uczyć się angielskiego , niemieckiego, chińskiego , itd. tym mniej czasu apka przydzieli na każde<br><br>
  
Algorytm K:<br>
  &nbsp;&nbsp;-apka proponuje zadanie o największym priorytecie (pierwsze z "góry")<br>
  &nbsp;&nbsp;-zadanie można odłożyć na później i apka zaproponuje następne o niższym priorytecie<br><br>
    
Minutnik:<br>
  &nbsp;&nbsp;-każde zadanie będzie musiało zawierać informację o postępie ({czas spędzony} / {czas spodziewany})<br>
  &nbsp;&nbsp;-apka musi wymagać dla zadań (innych niż stałe) żeby zatwierdzić zaczęcie i skończenie wykonywania zadania<br>
  &nbsp;&nbsp;-możliwość zrobienia przerwy - propozycja innego zadania<br>
  &nbsp;&nbsp;-przypomnienia o zadaniach stałych (np. siedzimy 30 minut nad matmą w domu, a za godzinę mamy być na wykładzie)<br><br>
  
Statystyki:<br>
  &nbsp;&nbsp;-anulowanie zadania przed zaczęciem bez kary<br>
  &nbsp;&nbsp;-przesunięcie zadania na później bez kary<br>
  &nbsp;&nbsp;-anulowanie niedkończonego zadania - karma Cię spotka<br>