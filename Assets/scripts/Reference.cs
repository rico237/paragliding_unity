using System.Collections;

public static class Reference {
	public static ArrayList blowables = new ArrayList();
	public const float MOUSE_SENSITIVITY = 3f;
	public const float PLAYER_WEIGHT = 80f;
	public const float GRAVITY = 9.807f;
	public const float LOOK_UP_LIMIT = 270f;
	public const float LOOK_DOWN_LIMIT = 90f;
	public const float FLYABLE_ANGLE = 30f;
	public const float PLAYER_MASS = PLAYER_WEIGHT*GRAVITY;
	public const float DRAG_COEFFICIENT_FRONT = 0.40f; //https://en.wikipedia.org/wiki/Drag_coefficient
	public const float DRAG_COEFFICIENT_UNDER = 0.9f;
	public const float DRAG_COEFFICIENT_SIDE = 0.50f;
	public const float DRAG_COEFFICIENT_PLAYER_FRONT = 0.4f;
	public const float AIR_DENSITY_20 = 1.2041f; //https://en.wikipedia.org/wiki/Density_of_air
	public const float AREA_UNDER = 22;
	public const float AREA_FRONT = 5.5f;
	public const float AREA_BRAKE = 2f;
	public const float AREA_SIDE = 2;
	public const float AREA_PLAYER_FRONT = 1;
	public const float STALL_LIMIT = 6;
	public const float SPEED_LIMIT = 3;
	public const float WIND_VOLUME = 1;
}
