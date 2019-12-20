using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class IKJoint : MonoBehaviour {

	public Vector3 offset;
	public Vector3 axis = Vector3.right;
	public float angle = 0f;
	public bool clampAngle = true;
	[Range(-180f, 0f)]
	public float minAngle = -120f;
	[Range(0f, 180f)]
	public float maxAngle = 120f;
	[Range(.1f, 10f)]
	public float lightness = 1f;

    void Start() {
		offset = transform.localPosition;
    }

    void Update() {
		Debug.Log(angle.ToString() + " " + axis.ToString());
        transform.localRotation = Quaternion.AngleAxis(angle, axis);
    }

	void OnDrawGizmos() {
		Gizmos.color = Color.cyan;
		Gizmos.DrawLine(transform.position, transform.position + transform.TransformDirection(axis) * 0.5f);
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(transform.position, .01f);
	}
}


[CustomEditor(typeof(IKJoint)), CanEditMultipleObjects]
public class IKJointEditor : Editor {
	
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		IKJoint ikjoint = (IKJoint)target;
		ikjoint.axis = ikjoint.axis.normalized;
		EditorGUILayout.MinMaxSlider(ref ikjoint.minAngle, ref ikjoint.maxAngle, -180f, 180f);
		//if (ikjoint.transform.parent != null) ikjoint.offset = ikjoint.transform.position - ikjoint.transform.parent.position;
	}
}
