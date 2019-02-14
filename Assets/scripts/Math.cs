using System.Collections;
using UnityEngine;

public class Math {
	public static Vector3 getDrag(Vector3 velocity, float dragCoefficient, float airDensity, float area){
		float sqrMagnitude = velocity.sqrMagnitude;
		return -velocity.normalized * 0.5f * dragCoefficient * airDensity * area * sqrMagnitude;
	}

	public static float GetWindPressure(float windSpeed){
		//P = 0.00256 x Pow(v,2) where v is speed in mph and times 0.44704 for mps.

		return 0.00256f * Mathf.Pow (windSpeed*0.44704f, 2);
	}

	public static Vector3 GetWindForce(Vector3 velocity, float area, float dragCoefficient){
		return area * GetWindPressure (velocity.magnitude) * dragCoefficient * velocity.normalized;
	}

	public static Vector3 GetWindVelocity(Vector3 windForce, float mass){ //Returns the 'velocity' of the wind
		return windForce * (1/mass) * Time.deltaTime;
	}
}

/*PHYSICS
 * v = velocity, t = time, a = average acceleration, F = force, m = mass
 * 
 * v = at
 * F = ma
 * 
 * Fd = drag, C = drag coefficient, p = air density, A = area exposed to air flow, v = velocity
 * 
 * Fd = ½*C*p*A*pow(2)
 * 
 * F = ma
 * a = vt
 * F = mvt
 * v = F/mt
 */

/*
 * -12 = -10*t
 * F = 80*10 = 800N TOTAL
 * 
 * // WindForce = A x P x Cd
		//A = exposed area, P = wind pressure, which is based on wind speed, Cd which is drag coefficient.
*/
