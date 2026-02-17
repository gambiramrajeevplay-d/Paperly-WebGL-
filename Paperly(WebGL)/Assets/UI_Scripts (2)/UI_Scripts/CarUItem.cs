using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarUItem : MonoBehaviour
{
    public GameObject root;        // Whole car UI (optional)
    public Image carImage;         // Rocket / car sprite
    public Button buyButton;       // Buy button
    public Button selectButton;    // Select button
    public int cost;               // Price
}
