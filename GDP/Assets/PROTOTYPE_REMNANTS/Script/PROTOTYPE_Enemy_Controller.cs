using UnityEngine;
using System.Collections;

public class PROTOTYPE_Enemy_Controller : MonoBehaviour
{
	private PROTOTYPE_Enemy_Movement enemyMovement;
	public int NumChildCells = 0;

	[SerializeField]
	private GameObject enemyCellPrefab;

	private void Awake()
	{
		enemyMovement = GetComponent<PROTOTYPE_Enemy_Movement>();
	}

	private void Start()
	{
		StartCoroutine(spawnChildCells());
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
			other.gameObject.GetComponent<PROTOTYPE_ChildController>().ReturnSquadManager().ReassignLeader();
            Destroy(other.gameObject);
        }
    }

	IEnumerator spawnChildCells()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(1f, 5f));

			GameObject newChild = (GameObject) Instantiate(enemyCellPrefab, transform.position, Quaternion.identity);
			newChild.transform.SetParent(this.transform);
			newChild.GetComponent<Rigidbody2D>().velocity = enemyMovement.thisRB.velocity;
			NumChildCells++;

			enemyMovement.changeSpeed(0.05f);
		}
	}
}
