using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolScript : MonoBehaviour
{
	public static ObjectPoolScript current;

	public Transform objectPooler;

	public List<ObjectTypes> ObjectsList = new List<ObjectTypes>();

	
	[System.Serializable]
	public struct ObjectTypes {  
		
		public GameObject pooledObject;
		public int pooledAmount;
		public bool willGrow;
		public List<GameObject> pooledObjects;
	}

	void Awake() {

		current = this;
	}

	void Start ()
	{
		for(int i = 0; i < ObjectsList.Count; i++)
		{
			for(int j = 0; j < ObjectsList[i].pooledAmount; j++)
			{
				GameObject obj = (GameObject)Instantiate(ObjectsList[i].pooledObject);
				obj.transform.SetParent(objectPooler);
				obj.SetActive(false);
				ObjectsList[i].pooledObjects.Add(obj);
			}
		}
	}
	
	public GameObject GetPooledObject(int k)
	{

		for(int i = 0; i< ObjectsList[k].pooledObjects.Count; i++)
		{
			if(ObjectsList[k].pooledObjects[i] == null)
			{
				GameObject obj = (GameObject)Instantiate(ObjectsList[k].pooledObject);
				obj.transform.SetParent(objectPooler);
				obj.SetActive(false);
				ObjectsList[k].pooledObjects[i] = obj;
				return ObjectsList[k].pooledObjects[i];
			}
			if(!ObjectsList[k].pooledObjects[i].activeInHierarchy)
			{
				return ObjectsList[k].pooledObjects[i];
			}    
		}
		
		if (ObjectsList[k].willGrow)
		{
			GameObject obj = (GameObject)Instantiate(ObjectsList[k].pooledObject);
			obj.transform.SetParent(objectPooler);
			ObjectsList[k].pooledObjects.Add(obj);
			return obj;
		}
	
		return null;
	}

}