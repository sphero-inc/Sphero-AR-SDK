using UnityEngine;
using System.Collections;

public static class VisionUtils
{
	//! @brief	number of vision puts before we switch to vision for heading
	public static int kPutCountForVisionHeading = 2;

	private static bool s_matricesInitialized = false;
	
	public static Matrix4x4 s_spheroToUnityTransform;

	public static Matrix4x4 SpheroToUnityTransform
	{
		get
		{
			if (!s_matricesInitialized)
			{
				InitializeMatrices();
			}
			return s_spheroToUnityTransform;
		}
	}

	private static void InitializeMatrices()
	{
		s_spheroToUnityTransform = Matrix4x4.TRS(
			Vector3.zero,
			Quaternion.AngleAxis(90.0f, Vector3.right),
			new Vector3(1.0f, 1.0f, 1.0f));
	}

	public static void AdjustSpheroTransformUsingHeadBob(ref Vector3 position, ref Quaternion orientation)
	{
#if USE_HEAD_BOB_CORRECTION
		position.x += headBobCameraOffset.x;
		position.z += headBobCameraOffset.z;
		//	The y part stays the same to keep the ball on the ground.
#endif
	}
	
	public static void GetCameraTransform(out Vector3 position, out Quaternion orientation)
	{
				
		ARUNBridge.ARUNGLMatrix matrix = ARUNBridge.CurrentARResult.CameraMatrix;

		//	Convert to matrix type so we can multiply.
		Matrix4x4 cameraTransform = new Matrix4x4();
		cameraTransform.SetRow(0, new Vector4(matrix.m11, matrix.m12, matrix.m13, matrix.m14));
		cameraTransform.SetRow(1, new Vector4(matrix.m21, matrix.m22, matrix.m23, matrix.m24));
		cameraTransform.SetRow(2, new Vector4(matrix.m31, matrix.m32, matrix.m33, matrix.m34));
		cameraTransform.SetRow(3, new Vector4(matrix.m41, matrix.m42, matrix.m43, matrix.m44));
		cameraTransform = SpheroToUnityTransform * cameraTransform;

		orientation = MatrixToQuaternion(ref cameraTransform);
		position = Translation(ref cameraTransform);
	}
	
	public static Quaternion MatrixToQuaternion(ref Matrix4x4 source)
	{
		// adapted from http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm

		Matrix4x4 matrix = new Matrix4x4();
		matrix.SetColumn(0, source.GetColumn(0));
		matrix.SetColumn(1, source.GetColumn(1));
		matrix.SetColumn(2, source.GetColumn(2));
		matrix.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

		float trace = Trace(ref matrix);
		if (trace > 0.0f)
		{
			float s = Mathf.Sqrt(trace + 1.0f) * 2.0f; // 4 * qw in Jon's equ
			return new Quaternion(
				(matrix[2,1] - matrix[1,2]) / s,
				(matrix[0,2] - matrix[2,0]) / s,
				(matrix[1,0] - matrix[0,1]) / s,
				0.25f * s);
		}
		else if (matrix[0,0] > matrix[1,1] && matrix[0,0] > matrix[2,2])
		{
			float s = Mathf.Sqrt(1.0f + matrix[0,0] - matrix[1,1] - matrix[2,2]) * 2.0f; // 4 * qx
			return new Quaternion(
				0.25f * s,
				(matrix[0,1] + matrix[1,0]) / s,
				(matrix[0,2] + matrix[2,0]) / s,
				(matrix[2,1] - matrix[1,2]) / s);
		}
		else if (matrix[1,1] > matrix[2,2])
		{
			float s = Mathf.Sqrt(1.0f + matrix[1,1] - matrix[0,0] - matrix[2,2]) * 2.0f; // 4 * qw
			return new Quaternion(
				(matrix[0,1] + matrix[1,0]) / s,
				0.25f * s,
				(matrix[1,2] + matrix[2,1]) / s,
				(matrix[0,2] - matrix[2,0]) / s);
		}
		else
		{
			float s = Mathf.Sqrt(1.0f + matrix[2,2] - matrix[0,0] - matrix[1,1]) * 2.0f; // 4 * qz
			return new Quaternion(
				(matrix[0,2] + matrix[2,0]) / s,
				(matrix[1,2] + matrix[2,1]) / s,
				0.25f * s,
				(matrix[1,0] - matrix[0,1]) / s);
		}
	}

	public static float Trace(ref Matrix4x4 matrix)
	{
		return matrix[0,0] + matrix[1,1] + matrix[2,2];
	}

	public static Vector3 Translation(ref Matrix4x4 matrix)
	{
		Vector4 translationCol = matrix.GetColumn(3);
		return new Vector3(translationCol.x, translationCol.y, translationCol.z);
	}

	public static Quaternion Normalize(Quaternion quat)
	{
		float length = Mathf.Sqrt(quat.x*quat.x + quat.y*quat.y + quat.z*quat.z + quat.w*quat.w);
		return new Quaternion(quat.x/length, quat.y/length, quat.z/length, quat.w/length);
	}

}
