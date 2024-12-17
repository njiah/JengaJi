using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    public GameObject blockPrefab;
    public int layers = 18;
    public float blockLength = 3f;
    public float blockWidth = 0.8f;
    public float blockHeight = 0.5f;    
    public Vector3 startPoint = Vector3.zero;

    void Start()
    {
        BuildTower();
        Debug.Log("Tower built!");
    }

    void BuildTower()
    {
        for (int layer=0; layer<layers; layer++)
        {
            for (int i=0; i<3; i++)
            {
                Vector3 position = startPoint;
                
                if (layer % 2 == 0)
                {
                    position += new Vector3(i*blockWidth, layer*blockHeight, 0);
                }
                else
                {
                    position += new Vector3(0, layer*blockHeight, i*blockWidth);
                }
                Instantiate(blockPrefab, position, Quaternion.Euler(0, (layer%2)*90, 0));   
            }
        }
    }
}
