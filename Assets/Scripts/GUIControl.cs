﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GUIControl : MonoBehaviour {

    public int rightBuffer = 150; //Right buffer in pixels (due to minimap)
    public int buttonWidth = 50; //Width in pixels
    public Button defaultButton;
    List<Button> buttons = new List<Button>();

    public void updateButtons(List<UnityAction> actions, List<string> actionNames)
    {
        while(buttons != null && buttons.Count > 0)
        {
            Button btn = buttons[0];
            Destroy(btn.gameObject);
            buttons.Remove(btn);
        }

        int lines = Mathf.CeilToInt(Screen.width / (buttonWidth * actions.Count));
        for(int i = 0; i < lines; i++)
        {
            for(int j = 0; j < Mathf.CeilToInt((float)actions.Count / lines) && i * lines + j < actions.Count; j++)
            {
                Button button = Instantiate<Button>(defaultButton, new Vector3(i,j), Quaternion.identity, transform);
                buttons.Add(button);
                Text text = button.GetComponentInChildren<Text>();
                text.text = actionNames[i*lines + j];
                button.onClick.AddListener(actions[i*lines + j]);
            }
        }
    }
}
