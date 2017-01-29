#include "SFML\Graphics.hpp"
#include <SFML/Audio.hpp>
#include <iostream>
#include <fstream>
#include <sstream>
#include <Windows.h>

using namespace sf;
using namespace std;

struct Converter
{
	const string characters = "#@$%&?!;:+-*',.";
	const float max = 11;
	string imgName;
	string txtName;
	float  accuracy;

	void Convert()
	{
		Image i;
		i.loadFromFile("IMG/" + imgName);
		ofstream myFile;
		myFile.open(txtName + ".txt");
		int x;
		for (int y = 0; y < i.getSize().y; y++)
		{
			for (x = 0; x < i.getSize().x; x++)
			{
				Color pixel = i.getPixel(x, y);
				float value = (float)pixel.r + (float)pixel.g + (float)pixel.b;
				float value2 = 10;
				int   counter = 0;
				char  character;

				while (value > value2 && counter < characters.length() - 1) {
					value2 += (max - accuracy) * 10;
					counter++;
				}

				myFile << characters[counter];
			}
			float percent = (((y) * (x)) / ((float)i.getSize().x * (float)i.getSize().y) * 100);
			cout << "Przetworzono: " << (y * x) << "/"  << (i.getSize().x * i.getSize().y) << " pikseli. (" << percent << "%)" << endl;
			myFile << endl;
		}
		myFile.close();
	}
};

int main()
{
	Converter converter;

	while (true)
	{
		system("cls");
		cout << "Podaj nazwe pliku graficznego, wraz z rozszerzeniem. (plik musi znajdowac sie w folderze IMG)\n";
		cin >> converter.imgName;
		cout << "Podaj nazwe pliku tekstowego. (bez rozszerzenia)\n";
		cin >> converter.txtName;
		cout << "Podaj konstast. (zalecana wartosc 4.5 - 6.5, zakres: 1.0-10.0)\n";
		cin >> converter.accuracy;

		if (converter.accuracy > 10 || converter.accuracy < 1){
			cout << "Wpisano zla wartosc, dokladnosc = 5\n";
			converter.accuracy = 5;
		}

		converter.Convert();

		cout << "Konwersja zakonczona\n";
		cin.clear();
		cin.ignore();
		cin.get();
	}
}