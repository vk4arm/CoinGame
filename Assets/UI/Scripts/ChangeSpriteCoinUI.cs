using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// Класс, который реализует смену спрайтов монеты пользовательского интерфейса
public class ChangeSpriteCoinUI : MonoBehaviour
{
    public float delay= 0.10f;
    public Sprite[] sprites;                    // Массив спрайтов
    private int nSprite = 0;                    // Номер текущего спрайта
    private Image image;                        // Изображение объекта

    // Функция, вызывается до отрисовки компонента
    void Start()
    {
        // Получаем ссылку на изображение объекта
        image = GetComponent<Image>();
        // Запускаем функция изменения изображения по таймеру
        InvokeRepeating("changeSprite", 0, delay);
    }

    // Функция цикличного изменения изображения
    void changeSprite()
    {
        nSprite = nSprite >= (sprites.Length - 1) ? 0 : nSprite + 1;
        image.sprite = sprites[nSprite];
    }
}
