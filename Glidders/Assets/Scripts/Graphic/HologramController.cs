using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

namespace Glidders
{
    namespace Graphic
    {
        public class HologramController : MonoBehaviour
        {

            [SerializeField] private CameraController cameraController;
            [SerializeField] private Tilemap tilemap;

            [SerializeField] private GameObject[] hologramPrefabs;
            private GameObject hologramObject;

            private const float MOVE_TIME = 0.2f;

            // Start is called before the first frame update
            void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }
            
            public void CreateHologram(int characterCode)
            {
                hologramObject = Instantiate(hologramPrefabs[characterCode]);
                hologramObject.SetActive(false);
            }

            public void DisplayHologram(Vector3 position, FieldIndex direction)
            {
                hologramObject.SetActive(true);
                hologramObject.transform.position = position;
                hologramObject.GetComponent<CharacterDirection>().SetDirection(direction);
                cameraController.AddTarget(hologramObject.transform);
            }
            public void DisplayHologram(FieldIndex position, FieldIndex direction)
            {
                hologramObject.SetActive(true);
                Vector3Int cellPosition = new Vector3Int(position.column, -position.row, 1);
                hologramObject.transform.position = tilemap.GetCellCenterWorld(cellPosition);
                hologramObject.GetComponent<CharacterDirection>().SetDirection(direction);
                cameraController.AddTarget(hologramObject.transform);
            }

            public void MoveHologram(Vector3 position, FieldIndex direction)
            {
                if (!hologramObject.activeSelf) return;
                Debug.Log(position);
                hologramObject.transform.DOMove(position, MOVE_TIME);
                hologramObject.GetComponent<CharacterDirection>().SetDirection(direction);
            }

            public void MoveHologram(FieldIndex position, FieldIndex direction)
            {
                if (!hologramObject.activeSelf) return;
                Vector3Int cellPosition = new Vector3Int(position.column, -position.row, 1);
                Vector3 movePosition = tilemap.GetCellCenterWorld(cellPosition);
                hologramObject.transform.DOMove(movePosition, MOVE_TIME);
                //Debug.Log(direction.row + ":" + direction.column);
                hologramObject.GetComponent<CharacterDirection>().SetDirection(direction);
            }

            public void SetHologramDirection(FieldIndex direction)
            {
                hologramObject.GetComponent<CharacterDirection>().SetDirection(direction);
            }

            public void DeleteHologram()
            {
                if (!hologramObject.activeSelf) return;
                hologramObject.SetActive(false);
                cameraController.RemoveTarget(hologramObject.transform);
            }
        }

    }
}
