using UnityEngine;
using System.Collections;

/*!
 * @brief	some math helpers reused between classes
 */
public static class ARUNMath
{
	public enum FrustumPlane
	{
		Left,
		Right,
		Bottom,
		Top,
		Far,
		Near
	}

	/*!
	 * @brief	transforms the given \a worldPoint apparent position in \a sourceCamera, to a new world point in the same apparent
	 *			position in \a destinationCamera
	 * @param 	worldPoint the point to transform
	 * @param 	sourceCamera the camera whose apparent position we're reading from
	 * @param 	destinationCamera a camera whose view will be used to compute a new world position
	 * @return	worldPoint as seen in \a sourceCamera in relation to \a destinationCamera's view frustum
	 */
	public static Vector3 TransformWorldPointBetweenCameras(Vector3 worldPoint, Camera sourceCamera, Camera destinationCamera)
	{
		if (sourceCamera == null || destinationCamera == null)
		{
			//DebugLogManager.LogError("A camera is NULL");
			return Vector3.zero;
		}

		Vector3 viewspacePoint = sourceCamera.WorldToViewportPoint(worldPoint);
		Vector3 destinationPoint = destinationCamera.ViewportToWorldPoint(viewspacePoint);
		return destinationPoint;
	}

	/*! 
	 * @brief	Generate a perspective transform matrix given the viewing frustum
	 * @param	left the left side of the viewing frustum
	 * @param	right the right side of the viewing frustum
	 * @param	top the top of the viewing frustum
	 * @param	bottom the bottom of the viewing frustum
	 * @param	near the near plane of the viewing frustum
	 * @param	far the far plane of the viewing frustum
	 * @return	the Matrix4x4 representing the described perspective projection matrix
	 */
	public static Matrix4x4 GeneratePerspective(float left, float right, float top, float bottom, float near, float far)
	{
		float rSubL = right - left;
		float tSubB = top - bottom;
		float fSubN = far - near;
		float twoN = 2.0f * near;
		
		Matrix4x4 perspective = new Matrix4x4();

		// setup row major
		perspective[0, 0] = twoN / rSubL;
		perspective[0, 1] = 0.0f;
		perspective[0, 2] = (right + left) / rSubL;
		perspective[0, 3] = 0.0f;

		perspective[1, 0] = 0.0f;
		perspective[1, 1] = twoN / tSubB;
		perspective[1, 2] = (top + bottom) / tSubB;
		perspective[1, 3] = 0.0f;

		perspective[2, 0] = 0.0f;
		perspective[2, 1] = 0.0f;
		perspective[2, 2] = -(far + near) / fSubN;
		perspective[2, 3] = -2.0f * far * near / fSubN;

		perspective[3, 0] = 0.0f;
		perspective[3, 1] = 0.0f;
		perspective[3, 2] = -1.0f;
		perspective[3, 3] = 0.0f;

		return perspective;
	}

	/*!
	 * @brief	generates a perspective projection matrix from the given platform configuration
	 * @param	config the platform configuration describing the camera
	 * @param	near the near plane
	 * @param	far the far plane
	 * @return	a new perspective projection matrix described by the given @a config
	 */
	public static Matrix4x4 PerspectiveProjectionFromCameraGeometry(ref ARUNBridge.AuPlatformConfiguration config, float far)
	{
		return GeneratePerspective(
			config.projFrustTop,
			-config.projFrustTop,
			-config.projFrustLeft,
			config.projFrustLeft,
			config.projFrustNear,
			far);
	}

	/*!
	 * @brief	creates a rotation matrix from a quaternion
	 * @param	quat the quaternion defining the rotation
	 * @return	a matrix that performs the same rotation as @a quat
	 */
	public static Matrix4x4 MatrixFromQuaternion(Quaternion quat)
	{
		// Referencing http://www.genesis3d.com/~kdtop/Quaternions-UsingToRepresentRotation.htm
		Matrix4x4 mat = new Matrix4x4();
		
		mat[0] = quat.x * quat.x - quat.y * quat.y - quat.z * quat.z + quat.w * quat.w;
		mat[1] = 2.0f * quat.x * quat.y + 2.0f * quat.z * quat.w;
		mat[2] = 2.0f * quat.x * quat.z - 2.0f * quat.y * quat.w;
		mat[3] = 0.0f;

		mat[4] = 2.0f * quat.x * quat.y - 2.0f * quat.z * quat.w;
		mat[5] = -quat.x * quat.x + quat.y * quat.y - quat.z * quat.z + quat.w * quat.w;
		mat[6] = 2.0f * quat.y * quat.z + 2.0f * quat.x * quat.w;
		mat[7] = 0.0f;

		mat[8] = 2.0f * quat.x * quat.y - 2.0f * quat.z * quat.w;
		mat[9] = 1.0f * quat.y * quat.z - 2.0f * quat.x * quat.w;
		mat[10] = -quat.x * quat.x - quat.y * quat.y + quat.z * quat.z + quat.w * quat.w;
		mat[11] = 0.0f;

		mat[12] = 0.0f;
		mat[13] = 0.0f;
		mat[14] = 0.0f;
		mat[15] = quat.x * quat.x + quat.y * quat.y + quat.z * quat.z + quat.w * quat.w;

		return mat;
	}

	/*!
	 * @brief	constructs a translation matrix about @a vec
	 * @param	vec the vector representing the translation
	 * @return	a matrix whose last column is @a vec
	 */
	public static Matrix4x4 MatrixFromOffset(Vector3 vec)
	{
		Matrix4x4 mat = new Matrix4x4();

		mat[0] = 1.0f;
		mat[1] = 0.0f;
		mat[2] = 0.0f;
		mat[3] = vec.x;

		mat[4] = 0.0f;
		mat[5] = 1.0f;
		mat[6] = 0.0f;
		mat[7] = vec.y;

		mat[8] = 0.0f;
		mat[9] = 0.0f;
		mat[10] = 1.0f;
		mat[11] = vec.z;

		mat[12] = 0.0f;
		mat[13] = 0.0f;
		mat[14] = 0.0f;
		mat[15] = 1.0f;

		return mat;
	}

	/*!
	 * @brief	creates a scaling matrix from the given @a scale
	 * @param	a scale to generate a matrix from
	 * @return	a matrix whose diagonal elements are @a scale
	 */
	public static Matrix4x4 MatrixFromScale(Vector3 scale)
	{
		Matrix4x4 mat = new Matrix4x4();

		mat[0] = scale.x;
		mat[1] = 0.0f;
		mat[2] = 0.0f;
		mat[3] = 0.0f;

		mat[4] = 0.0f;
		mat[5] = scale.y;
		mat[6] = 0.0f;
		mat[7] = 0.0f;

		mat[8] = 0.0f;
		mat[9] = 0.0f;
		mat[10] = scale.z;
		mat[11] = 0.0f;

		mat[12] = 0.0f;
		mat[13] = 0.0f;
		mat[14] = 0.0f;
		mat[15] = 1.0f;

		return mat;
	}

	public static Plane[] FrustumFromCamera(Camera camera)
	{
		Vector4[] Rows = new Vector4[4];
		Matrix4x4 worldProjection = camera.projectionMatrix;
		for (int i = 0; i < 4; i++)
		{
			Rows[i] = worldProjection.GetRow(i);
		}

		Plane[] FrustumPlanes = new Plane[6];

		FrustumPlanes[(int)FrustumPlane.Left] = PlaneFromVector4(Rows[3] + Rows[0]);
		FrustumPlanes[(int)FrustumPlane.Right] = PlaneFromVector4(Rows[3] - Rows[0]);

		FrustumPlanes[(int)FrustumPlane.Top] = PlaneFromVector4(Rows[3] + Rows[1]);
		FrustumPlanes[(int)FrustumPlane.Bottom] = PlaneFromVector4(Rows[3] - Rows[1]);

		FrustumPlanes[(int)FrustumPlane.Far] = PlaneFromVector4(Rows[3] + Rows[2]);
		FrustumPlanes[(int)FrustumPlane.Near] = PlaneFromVector4(Rows[3] - Rows[2]);

		// view space
		return FrustumPlanes;
	}

	/*!
	 * @brief	performs a very simple debug visualization of a plane (drawin gthe normal red and two perpendicular vectors)
	 * @param	planes a list of planes to draw(as would be generated from frustum functions)
	 */
	public static void DrawPlanes(Plane[] planes)
	{
		foreach(Plane p in planes)
		{
			Vector3 position = -p.normal * p.distance;
			Debug.DrawLine(position, position + p.normal, Color.red);

			Vector3 right = Vector3.Cross(p.normal, Vector3.up).normalized;
			Debug.DrawLine(position, position + right, Color.green);

			Vector3 up = Vector3.Cross(right, p.normal).normalized;
			Debug.DrawLine(position, position + up, Color.blue);
		}
	}

	/*!
	 * @brief	intersects a ray with the given frustum
	 * @param	ray the ray to intersect
	 * @param	frustum the frustum to intersect (should be a number of planes [6] completely enclosing the space)
	 * @param[out]	forward used to store the closest point along the positive direction of the ray
	 * @param[out]	backward used to store the closest point along teh negative direction of the ray
	 */
	public static void IntersectRayWithFrustum(
		Ray ray,
		Plane[] frustum,
		out Vector3 forward,
		out Plane forwardPlane,
		out Vector3 backward,
		out Plane backwardPlane)
	{
		float closestForwards = float.MaxValue;
		Plane closestForwardPlane = frustum[0];
		float closestBackwards = -float.MaxValue;
		Plane closestBackwardPlane = frustum[0];
		foreach (Plane plane in frustum)
		{
			float distance;
			bool hitForward = plane.Raycast(ray, out distance);
			if (hitForward)
			{
				if (distance < closestForwards)
				{
					closestForwards = distance;
					closestForwardPlane = plane;
				}
			}
			else
			{
				if (distance < 0.0f && distance > closestBackwards)
				{
					closestBackwards = distance;
					closestBackwardPlane = plane;
				}
			}
		}

		forward = ray.GetPoint(closestForwards);
		forwardPlane = closestForwardPlane;
		backward = ray.GetPoint(closestBackwards);
		backwardPlane = closestBackwardPlane;
	}

	/*!
	 * @brief	given a vector where each component corresponds to a plane component, converts the vector to a plane
	 */
	private static Plane PlaneFromVector4(Vector4 plane)
	{
		return new Plane(new Vector3(plane.x, plane.y, plane.z), plane.w);
	}

	/*!
	 * @brief	converts the view frustum to local space
	 * @param	frustum a view frustom to transform
	 * @param	localSpace a Transform defining local space
	 * @return	the view frustum in local space
	 */
	public static Plane[] FrustumToLocalSpace(Plane[] frustum, Transform localSpace)
	{
		Plane[] outFrustum = new Plane[frustum.Length];
		for (int i = 0; i < frustum.Length; i++)
		{
			outFrustum[i] = PlaneToLocalSpace(frustum[i], localSpace);
		}
		return outFrustum;
	}

	/*!
	 * @brief	converts a plane form world to local space
	 * @param	plane the plane to convert
	 * @param	localSpace the Transform defining localSpace
	 * @return	the plane in local space
	 */
	public static Plane PlaneToLocalSpace(Plane plane, Transform localSpace)
	{
		Vector3 normal = plane.normal;
		Vector3 point = -plane.normal * plane.distance;
		normal = localSpace.InverseTransformDirection(normal);
		point = localSpace.InverseTransformPoint(point);
		return new Plane(normal, point);
	}

	/*!
	 * @brief	Vector3 has no division operator for float/vector. This is dumb
	 * @param	f the float of the numerator
	 * @param	vec the vector for the denominator
	 * @return	@a vec scaled by @a f / @a vec
	 */
	public static Vector3 Division(float f, Vector3 vec)
	{
		return new Vector3(f/vec.x, f/vec.y, f/vec.z);
	}
}
