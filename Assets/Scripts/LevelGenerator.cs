using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public int width;                           // Ширина уровня
    public int height;                          // Высота уровня
    public GameObject playerPrefab;             // Префаб игрока
    public GameObject basePrefab;               // Префаб базы противника
    public GameObject wallPrefab;               // Префаб обычной стены
    public GameObject edgeWallPrefab;           // Префаб стены на краю уровня
    public GameObject enemyPrefab;              // Префаб противника

    public int wallCount = 10;                   // Количество обычных стен на уровне
    public int enemyCount = 5;                   // Количество противников

    private GameObject[,] levelGrid;             // Сетка уровня

    private void Start()
    {
        GenerateLevel();
    }

    private void GenerateLevel()
    {
        // Создаем пустой уровень
        levelGrid = new GameObject[width, height];

        // Размещаем базу внизу посередине
        Vector2Int basePosition = new Vector2Int(width / 2, 1);
        Instantiate(basePrefab, new Vector3(basePosition.x, basePosition.y, 0), Quaternion.identity);
        levelGrid[basePosition.x, basePosition.y] = basePrefab;

        // Размещаем игрока возле базы
        Vector2Int playerPosition = GetAdjacentEmptyPosition(basePosition);
        if (playerPosition != Vector2Int.one * -1)
        {
            Instantiate(playerPrefab, new Vector3(playerPosition.x, playerPosition.y, 0), Quaternion.identity);
            levelGrid[playerPosition.x, playerPosition.y] = playerPrefab;
        }

        // Размещаем противников на противоположной стороне базы
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2Int enemyPosition = GetOppositeEmptyPosition(basePosition);
            if (enemyPosition != Vector2Int.one * -1)
            {
                Instantiate(enemyPrefab, new Vector3(enemyPosition.x, enemyPosition.y, 0), Quaternion.identity);
                levelGrid[enemyPosition.x, enemyPosition.y] = enemyPrefab;
            }
        }

        // Размещаем стены на краях уровня
        for (int x = 0; x < width; x++)
        {
            Instantiate(edgeWallPrefab, new Vector3(x, 0, 0), Quaternion.identity);
            Instantiate(edgeWallPrefab, new Vector3(x, height - 1, 0), Quaternion.identity);
            levelGrid[x, 0] = edgeWallPrefab;
            levelGrid[x, height - 1] = edgeWallPrefab;
        }

        // Размещаем стены по бокам уровня
        for (int y = 1; y < height - 1; y++)
        {
            Instantiate(edgeWallPrefab, new Vector3(0, y, 0), Quaternion.identity);
            Instantiate(edgeWallPrefab, new Vector3(width - 1, y, 0), Quaternion.identity);
            levelGrid[0, y] = edgeWallPrefab;
            levelGrid[width - 1, y] = edgeWallPrefab;
        }

        // Размещаем обычные стены
        for (int i = 0; i < wallCount; i++)
        {
            Vector2Int wallPosition = GetRandomEmptyPosition();
            if (wallPosition != Vector2Int.one * -1)
            {
                Instantiate(wallPrefab, new Vector3(wallPosition.x, wallPosition.y, 0), Quaternion.identity);
                levelGrid[wallPosition.x, wallPosition.y] = wallPrefab;
            }
        }
    }

    private Vector2Int GetAdjacentEmptyPosition(Vector2Int position)
    {
        List<Vector2Int> adjacentPositions = new List<Vector2Int>();

        // Проверяем соседние позиции сверху, снизу, слева и справа
        Vector2Int up = new Vector2Int(position.x, position.y + 1);
        Vector2Int down = new Vector2Int(position.x, position.y - 1);
        Vector2Int left = new Vector2Int(position.x - 1, position.y);
        Vector2Int right = new Vector2Int(position.x + 1, position.y);

        if (IsPositionEmpty(up))
            adjacentPositions.Add(up);
        if (IsPositionEmpty(down))
            adjacentPositions.Add(down);
        if (IsPositionEmpty(left))
            adjacentPositions.Add(left);
        if (IsPositionEmpty(right))
            adjacentPositions.Add(right);

        if (adjacentPositions.Count == 0)
            return Vector2Int.one * -1; // Возвращаем недопустимую позицию, если нет доступных соседей

        int randomIndex = Random.Range(0, adjacentPositions.Count);
        return adjacentPositions[randomIndex];
    }

    private Vector2Int GetOppositeEmptyPosition(Vector2Int position)
    {
        Vector2Int oppositePosition = GetOppositePosition(position);

        if (IsPositionEmpty(oppositePosition))
            return oppositePosition;

        return Vector2Int.one * -1; // Возвращаем недопустимую позицию, если противоположная позиция занята
    }

    private bool IsPositionEmpty(Vector2Int position)
    {
        if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height)
        {
            return levelGrid[position.x, position.y] == null;
        }

        return false;
    }

    private Vector2Int GetRandomEmptyPosition()
    {
        List<Vector2Int> emptyPositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (levelGrid[x, y] == null)
                {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (emptyPositions.Count == 0)
        {
            return Vector2Int.one * -1; // Возвращаем недопустимую позицию, если не найдено пустых мест
        }

        int randomIndex = Random.Range(0, emptyPositions.Count);
        return emptyPositions[randomIndex];
    }

    private Vector2Int GetOppositePosition(Vector2Int position)
    {
        int oppositeX = width - 1 - position.x;
        int oppositeY = height - 1 - position.y;
        return new Vector2Int(oppositeX, oppositeY);
    }
}