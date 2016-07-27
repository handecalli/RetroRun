using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputNavigator : MonoBehaviour {

	EventSystem system;

	
	void Start()
	{
		system = EventSystem.current;
		
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

			if (next != null)
			{
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null)
					inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
				
				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			} 
			
			else
			{
				next = Selectable.allSelectables[0];
				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
			
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

			if (next.name == "btnLogin")
			{
				next.Select();
			}
		}
	}
}
