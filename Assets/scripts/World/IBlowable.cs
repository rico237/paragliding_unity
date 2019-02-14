using UnityEngine;
using System.Collections;

public interface IBlowable{

	//Extremely simple interface for blowable objects (can be affected by wind).
	void AddWind (Vector3 wind);

	Vector3 GetWorldPosition();
}
