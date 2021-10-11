using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	public ArrayLayout data;

    public void Start()
    {
        Debug.Log(data.rows[0].row[0].name);
    }
}
