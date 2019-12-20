using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematics : MonoBehaviour {
    
	public IKJoint[] ikjoints;
	public Transform target;

	const float sampleAngle = 1f;
	const float learnRate = 25f;
	const float distThreshold = 0.05f;
	[Range(0f, 2f)]
	public float distanceWeight = 1f;
	[Range(0f, 2f)]
	public float rotationWeight = 1f;
	[Range(0f, 2f)]
	public float torsionWeight = 1f;


    void Start() {
        //Debug.Log(EndPoint());
		//Debug.Log(PartialDerivative(0));
    }

    void Update() {
		for (int i = 0; i < 10; i++) {
        	RunInverseKinematics();
		}
    }

	public Vector3 EndPoint() {
		Vector3 prevPos = ikjoints[0].transform.position;
		Quaternion rot = Quaternion.identity;

		for (int i = 1; i < ikjoints.Length; i++) {
			IKJoint prevJoint = ikjoints[i - 1];
			rot *= Quaternion.AngleAxis(prevJoint.angle, prevJoint.axis);
			Vector3 pos = prevPos + rot * ikjoints[i].offset;
			prevPos = pos;
		}
		return prevPos;
	}

	public Quaternion EndAngle() {
		Quaternion rot = Quaternion.identity;

		for (int i = 0; i < ikjoints.Length; i++) {
			IKJoint joint = ikjoints[i];
			rot *= Quaternion.AngleAxis(joint.angle, joint.axis);
		}
		return rot;
	}

	public float DistanceToTarget() {
		return Vector3.Distance(EndPoint(), target.position);
	}

	public float RotationToTarget() {
		return Quaternion.Angle(EndAngle(), target.rotation);
	}

	public float NetTorsion() {
		float torsion = 0f;
		int clampedJoints = 0;
		for (int i = 0; i < ikjoints.Length; i++) {
			IKJoint joint = ikjoints[i];
			if (joint.clampAngle) {
				clampedJoints++;
				torsion += Mathf.Abs(joint.angle);
			}
		}
		torsion /= clampedJoints;
		return torsion;
	}

	public float NetError(int index) {

		float s2inv = 1 / (sampleAngle * 2);

		float originalAngle = ikjoints[index].angle;
		ikjoints[index].angle = originalAngle + sampleAngle;
		float dp = DistanceToTarget();
		float rp = RotationToTarget();
		float tp = NetTorsion();
		ikjoints[index].angle = originalAngle - sampleAngle;
		float dm = DistanceToTarget();
		float rm = RotationToTarget();
		float tm = NetTorsion();
		ikjoints[index].angle = originalAngle;

		float distanceError = (dp - dm) * s2inv;
		distanceError = Mathf.Clamp(distanceError * 25f, -1, 1);
		distanceError *= distanceWeight;

		float rotationError = (rp - rm) * s2inv;
		rotationError = Mathf.Clamp(rotationError / 180f * 10f, -1, 1);
		rotationError *= rotationWeight;
		
		float torsionError = (tp - tm) * s2inv;
		torsionError = Mathf.Clamp(torsionError / 180f * 10f, -1, 1);
		torsionError *= torsionWeight;

		return
			distanceError * DistanceToTarget()
			+ rotationError * RotationToTarget() / 10f
			+ torsionError * NetTorsion() / 10f
		;
		
	}

	public void RunInverseKinematics() {

		float currentDist;

		for (int i = ikjoints.Length - 1; i >= 0; i--) {

			currentDist = DistanceToTarget();
			//if (currentDist < distThreshold) return;

			IKJoint joint = ikjoints[i];

			joint.angle -= NetError(i) * learnRate * joint.lightness;
			if (joint.clampAngle) joint.angle = Mathf.Clamp(joint.angle, joint.minAngle, joint.maxAngle);
		}
	}

	void OnDrawGizmos() {
		//Gizmos.color = Color.green;
		//Gizmos.DrawSphere(target.position, .15f);
	}
}
