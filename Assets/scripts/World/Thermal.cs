using UnityEngine;
using System.Collections;

public class Thermal : MonoBehaviour, IBlowable {

    ArrayList inThermal;

	// Use this for initialization
	void Start () {
        inThermal = new ArrayList();

        Reference.blowables.Add(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        foreach(IBlowable blowable in inThermal)
        {
            blowable.AddWind(GetThermalWind());
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        IBlowable blowable = (IBlowable)collider.GetComponent(typeof(IBlowable));

        inThermal.Add(blowable);
    } 

    void OnTriggerExit(Collider collider)
    {
        IBlowable blowable = (IBlowable)collider.GetComponent(typeof(IBlowable));

        inThermal.Remove(blowable);
    }
    
    public Vector3 GetVelocity(IBlowable obj)
    {
        if (inThermal.Contains(obj))
        {
            return GetThermalWind();
        }else
        {
            return Vector3.zero;
        }
    }

    public void AddWind(Vector3 wind)
    {
        //I don't know if this simulates the correct wind, but it works pretty well for now
        transform.position += wind*Time.deltaTime;
    }

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    private Vector3 GetThermalWind()
    {
        return Vector3.up * 5;
    }
  
}
