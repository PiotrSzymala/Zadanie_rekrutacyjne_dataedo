# ConsoleApp
Do napisanego kodu załączam Readme, w którym opisze swoje założenia podczas tworzenie tego rozwiązania.

## Wymagane zmiany
1. **Poprawnie przetwarzać każdy przypadek testowy**
    - Błąd przetwarzania wynikał z róznej ilości nagłówków pomiędzy plikami (sampleFile3.csv miał ich mniej niż pozostałe). Początkowy kod tego nie uwzględniał. W swoim rozwiązaniu wziąłem pod uwagę, że pierwsza linijka danych to zawsze informacja, jakie pola znajdują się w pliku i na ich podstawie mapowałem dane. 
3. **Poprawić wyświetlanie obiektów**
    - Puste opisy, schemy oraz tytuły (puste linie, kropki, które nic nie poprzedza oraz puste nawiasy nie są już wyświetlane).
    - Termy nie były matchowane z powodu niodpowiedniego typu względem sourceFile.csv. Areas przede wszystkim dlatego, że 3 plik, w którym głównie się znajdowały nie był poprawnie przetwarzany. 
4. **Dodać logger i wypisać w nim nieprzetworzone linie**
    - Skorzystałem z Nloga i zapewniłem logowanie konrketnej lini z numerem, jeśli brakowało jakiejś wartości podczas przetwarzania. Dodatkowo wprowadziłem wielopoziomowe logowanie błędów, warningi i errory tworzą się w osobnych folderach oraz zastosowałem podział logów ze względu na dni (foldery z datami oraz nazwy plików z konkretną godziną i minutą).   
5. **Performance i czytelność kodu**
    - Początkowy kod nie spełniał zasad SOLID, więc został przeze mnie wydzielony do osobnych klas i odpowiednio zrefaktorowany. Stworzyłem strukturę projektu z podziałem na Interfejsy, Modele, Serwisy oraz Utilities. Utworzyłem dokumentację XML do swoich metod. Korzystałem z interfejsów, które zapewniły modularność oraz ułatwiły testowanie kodu (dodatkowo napisałem parę testów)

## Moje uwagi
- Przy wyświetlaniu danych, celowo zostawiłem linę odstępu pomiędzy głównymi obiektami dla lepszej czytelności.
- Nie do końca jestem pewny w kwestii termów czy udało mi się to dobrze zrobić, aczkolwiek na początku było bardzo dużo nullów w matchach, a w moim końcowym kodzie nie ma żadnego.  
