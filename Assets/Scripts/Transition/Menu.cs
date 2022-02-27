using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static int _size;
    public static int _difficulty;
    public void Generate()
    {
        _size = (int)GameObject.Find("Size").GetComponent<Slider>().value;
        _difficulty = (int)GameObject.Find("Difficulty").GetComponent<Slider>().value;
        SceneManager.LoadScene("Algorithm");
    }
}
