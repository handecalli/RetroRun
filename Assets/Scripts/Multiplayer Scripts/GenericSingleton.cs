/*
 *
 *  * Copyright (c) 2015 no-pact.
 *  * All rights reserved.
 *  * no-pact PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 *
 */

using UnityEngine;
using System.Collections;

public class GenericSingleton<T> where T : GenericSingleton<T>, new(){

	protected bool initialized;

	private static volatile T instance;
	private static object syncLock = new System.Object();
	
	public static T Instance
	{
		get
		{
			if (instance == null) {
				
				lock (syncLock) {
					
					if (instance == null) {
						
						instance = new T();

						//initialize instance if necessary
						if (!instance.initialized) {
							
							instance.Initialize ();
							instance.initialized = true;
							
						}
					}
				}
				
			}
			return instance;
		}
	}

	protected virtual void Initialize ()
	{
        
    }

}
