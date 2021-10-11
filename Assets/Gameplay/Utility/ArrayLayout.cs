using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout  {

	[System.Serializable]
	public struct rowData{
		public GameObject[] row;
	}

	public rowData[] rows = new rowData[3]; //Grid of 3x3
    
}
