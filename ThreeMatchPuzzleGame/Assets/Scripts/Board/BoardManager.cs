using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Numerics;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using System.Linq;


/// <summary>
/// ���콺 �̵������� ����ϴ� ������
/// </summary>
public enum MouseMoveDirection
{
     MOUSEMOVERIGHT,
     MOUSEMOVELEFT,
     MOUSEMOVEUP,
     MOUSEMOVEDOWN
};

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject _BlockPrefab;

    //��������Ʈ �̹��� �迭�� ����� (�������� �̹����� �ֱ� ���ؼ�) : 13�� �̹����� �ν�����â �迭�� �ִ´�.
    //�̹����� �����Ǿ� �ִ� ��쿡�� �ν�����â�� ���� �ְ� �������̸� �ҷ����̴� ���� ���� �� �ִ�.
    [SerializeField] private Sprite[] _Sprites; 


    private Vector3 _screenPos; //��ũ�� 0,0���� ���� ��ǥ�� ����
    float _ScreenWidth; //��ũ�� ����
    float _BlockWidth;  //�� �ϳ��� ���̸� �����ϱ� ���� ����
    //float _ScreenHeight;    //��ũ�� ����
    //����ó�� (2�� �ʿ���(x,y)
    private float _Xmargin = 0.5f; //x�� ����
    private float _Ymargin = 2f; //y�� ����

    float _Scale = 0f;  //���� �����ϰ� ����
    [SerializeField] private GameObject Parent;

    //�� ������ ������ ( ��ư ��ũ��Ʈ�� �� �����̴� �뵵) : 2���� �迭 ���
    public GameObject[,] _GameBoard;

    void Start()
    {
        //��ũ��0,0��ǥ�� ���� �������� ��ǥ������ ��ȯ
        _screenPos = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 10f));
        //+
        _screenPos.y = -_screenPos.y;
        Debug.Log(_screenPos);

        //��ũ���� ���̴� �Ʊ� ���� ��ũ�� 0,0 ��ǥ�� x ���� ��( x ��ǥ�� - �̱� ������ ������ ���Ѵ�)
        _ScreenWidth = Mathf.Abs(_screenPos.x * 2);
        //�� �������� ���� �� ������Ʈ�� �ִ� �� �̹��� ������Ƽ�� �����ؼ� ���� ����� �����´�
        //�ȼ����� 100���� ������ ���� : ����Ƽ�󿡼� �ȼ� 100�� 1��. size.x �� �ȼ��������ε� �̰� 100���� ������
        //3d �������� ����Ƽ ����(����)�� �ٲ۰��̴�. �ȼ��� 1000�� ��� 3d�������� ���̴� 10�� �ȴ�.
        // pixel per unit (100�ȼ��� 1����) => 1���ͷ� ����� �ȴ�.=> 100���� ������ ����Ƽ�� ������ �����
        _BlockWidth = _BlockPrefab.GetComponent<Block>().BlockImage.sprite.rect.size.x/100;




        MakeBoard(5,5);
    }
    /// <summary>
    /// ����ġ instantiate -> transform.position ->
    /// </summary>
    /// <param name="column"></param>
    /// <param name="row"></param>
    /// 

    //Updata ������ ���� ���Ե� ���� ��ǥ�� ��� ���� ����� ����ʵ� ( �÷��� �ο츸ŭ for���� ���� �� ���)
    private int _Column = 0;
    private int _Row = 0;

    void MakeBoard(int column, int row)
    {
        float width = _ScreenWidth - (_Xmargin * 2); //��µǴ� �ʺ� ( 5.6�̵�. ��ũ�� x �� 2.8�̰� ��ü ��ũ�� �ʺ� ���� ��)
        //=> x ������ ���ʿ� �ֱ� ������ ���ϱ� 2 ���ش� (y�� �ϳ��� �������ָ� ��)

        //���� �ʺ� �ϳ��� �����ϳ� ��ü �ʺ�� row �� ������ ���� �޶�����
        float blockWidth = _BlockWidth * row;   //���� ��ü ��� �ʺ� (���� row���� ���� ��ü ����� �޶�����)
        //���� ������ ���� �����ؾ��Ѵ� => ���� 5,5 �� ��ũ�� ȭ�� �ȿ� ����� ������ 10,10 �̸� ȭ�� ������ �Ѿ��
        //=> �ֳĸ� �� �ʺ�� row ���� ���� �޶����� ������
        //��ũ�� �ʺ� ���� �� ���� ��������? ���ʺ�� ��ü �ʺ�� ������ ������ ���� ���Ѵ� -> �� ������ ����
        _Scale = width / blockWidth; //���� ������ �� 

        //��� �� ������ ���� ���带 ����� ( �迭�� ũ�⸦ ������)
        _GameBoard = new GameObject[column, row];

        //���� col��row��  ����Ͽ� ���� ��ǥ�� �˾Ƴ���
        _Column = column;
        _Row = row;

        for (int co = 0; co < column; co++)
        {
            for (int ro = 0; ro < row; ro++)
            {
                //���� ������ �޾Ƽ� Instantiate �Ѵ�.
                //GameObject blockObj = Instantiate(_BlockPrefab);
                //���� �� �����ϰ����� ���� �������� �������ش�

                //2���� �迭�� �ٲ��ش�
                _GameBoard[co,ro] = Instantiate(_BlockPrefab);

                //�������ؼ� �θ� ����
                _GameBoard[co, ro].transform.SetParent(Parent.transform);

                _GameBoard[co, ro].transform.localScale = new Vector3(_Scale, _Scale, 0f);
                //���� �����ϰ��� ���������� ��ũ���� �� �������� ����.


                //���� ���� �ڸ��� ��������� ������ ��ġ�� �������ش� (����������)
                //�� ó�� ��ġ�κ��� �����ϸ鼭 ��������� ������ �������� ���� ����
                //�츮�� ���ϴ°� ���ϴ����� ������ ���̱� ������ y���� -�� �ش�
                //screenpos�� 0.0 ��ǥ�� �˾Ƴ� ���� �»�� ��ǥ�� �����ش�
                //���ʿ� ro,co ù��° ��ǥ�� 1�̱� ������ ������ �������� ����
                //�׸��� 0.5f �����ؼ� ȭ�鿡 ������ �� (���� �߸��� �ʰ�)
                //blockObj.transform.position = new Vector3(_screenPos.x+ ro + 0.5f, _screenPos.y-co-0.5f, 0.0f);

                //blockwidth ���� blockscale���� �������ش�
                //blockObj.transform.position = new Vector3(_screenPos.x + ro * (_BlockWidth * _Scale) + 0.5f, 
                //                                          _screenPos.y - co * (_BlockWidth * _Scale) - 0.5f, 0.0f);

                //screenpos�� �»���ε� x������ ���� => x������ŭ �������� ��µȴ� (�ű⼭���� �� ��ġ)
                //0.5�� ���Ƿ� ��ġ�ߴµ� ���� ������ ���� ������ �޶��� ( ���� ������ ���� Ŀ���� �۾����� �ϱ� ������
                //�������� �����ش� (_BlockWidth*_Scale/2) => x �ุŭ ����
                //y������ ������ �Ʒ��� �������� ������ ���̳ʽ� ���ش�. 
                _GameBoard[co, ro].transform.position = new Vector3(_screenPos.x + _Xmargin + ro * (_BlockWidth * _Scale) + (_BlockWidth *_Scale)/2f,
                                          _screenPos.y - _Ymargin - co * (_BlockWidth * _Scale) - (_BlockWidth * _Scale)/2f, 0.0f);

                //�� ��ũ��Ʈ�� �ִ� width���ٰ� ���� �ʺ� �����Ѵ� (�Űܾ� �ϱ� ����)
                _GameBoard[co, ro].GetComponent<Block>().Width = (_BlockWidth * _Scale); //���� �ʺ��� ���� ����
                _GameBoard[co, ro].GetComponent<Block>().MovePos = _GameBoard[co, ro].transform.position;   //�� ���� ��ġ���� ����

                //_GameBoard[co, ro].GetComponent<Block>().Move(DIRECTION.LEFT);

                //���� �̸��� ���� (����� �������� Ȯ��)
                _GameBoard[co, ro].name = $"Block[{co},{ro}]";

                //���� �÷��� �ο쿡 ���� �����Ѵ� ( �� ó�� ��������� �� ���� �ο���)
                //=> Ŭ���� ������Ʈ�� �˰� ������ �� ���� ���� �ִ��� �˾ƾ� �ϱ� ������.
                //���� �÷��� �ο찪�� �˸� ���� ���� �ִ��� �� �� ����
                _GameBoard[co,ro].GetComponent<Block>().Column = co;
                _GameBoard[co,ro].GetComponent<Block>().Row = ro;

                //������ ���� ���������� ����Ƽ ������ ���� �Լ��� ����Ѵ� => �پ��� �̹��� �ֱ�
                int type = UnityEngine.Random.Range(0,5 /*_Sprites.Length*/); //0���� 12 ������ ��ȣ�� �����ͼ� �������ش�
                _GameBoard[co,ro].GetComponent<Block>().Type = type;    //Ÿ���� �������ش�
                _GameBoard[co,ro].GetComponent<Block>().BlockImage.sprite = _Sprites[type]; //��������Ʈ���ٰ� Ÿ�Թ�ȣ�� �־��ش�
                //�� �̹����� ��ü�Ѵ�.

            }
        }
    }
    #region uiButton
    public void MoveBlockLeft()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.LEFT);
        }
    }

    public void MoveBlockRight()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.RIGHT);
        }
    }

    public void MoveBlockUp()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.UP);
        }
    }

    public void MoveBlockDown()
    {
        foreach (var obj in _GameBoard)
        {
            obj.GetComponent<Block>().Move(DIRECTION.DOWN);
        }
    }
    #endregion

    //���� ������ ���� �� ���ϱ�
    //���콺�� Ŭ������ �� ���� ��ġ�� ���ϴ� start ������ �� ���� ��ġ�� ���ϴ� end
    private Vector3 _StartPos = Vector3.zero;   //���콺 Ŭ�� �� ������ǥ
    private Vector3 _EndPos = Vector3.zero; //���콺 Ŭ�� ���� ��ǥ
    bool _IsOver = false;   //Ŭ���� ���� �ִ��� ����
    //Ŭ���� ������Ʈ ����
    GameObject _ClickObject;    //Ŭ���� �� ������ ����
    bool _MouseClick = false;   //���콺�� Ŭ���������� �ƴ��� üũ
    float _MoveDistance = 0.01f; //�ΰ��� ����(���������� ���� �Ÿ��� �Ǵ� ���ذ�)

    /// <summary>
    /// Ŭ���� ���¿��� ���콺 �̵� �� ȣ��
    /// </summary>
    void MouseMove()
    {
        //Debug.Log("MouseMove");

        //����üũ
        //�� ���ͻ����� �Ÿ��� ���Ѵ�
        float diff = Vector2.Distance(_StartPos, _EndPos);
        //���� 
        //������ �Ÿ��� ���� ������ MoveDistance ���� Ŭ �� ó��, MoveDistance : ���� 0.01f
        // Ŭ���� ������Ʈ�� ���� ��
        //Debug.Log("Diff " +diff);
        //Debug.Log("���� " + _MoveDistance);

        //�� ������ �Ÿ��� �������� ũ�� + Ŭ���� ������Ʈ�� ���� �ƴϰ� + �Է� ������ ��
        if(diff > _MoveDistance && _ClickObject != null && _InputOK == true) 
        {
            //����� �������� �������� �Ͼ���� ���
            // ���콺 ���� enum, ���콺 �̵� �� ������ ����ϴ� �Լ�, ���� ��� �ڿ� �� ������ ������ ���ϴ� �Լ�
            MouseMoveDirection dir = CalculateDirection();
            Debug.Log("Direction "+ dir);
            //������ ���� �Է�ó���� �ȵǰ� ����
            _InputOK = false;

            switch(dir) 
            {
                case MouseMoveDirection.MOUSEMOVELEFT:
                    {
                        //���� Ŭ���� ���� ��� �� ���� �� ������Ʈ���� ������ �´�
                        int column = _ClickObject.GetComponent<Block>().Column;
                        int row = _ClickObject.GetComponent<Block>().Row;
                        //���� �ٲ� �� ���� �ȿ����� �ٲ��� �Ѵ�

                        //������ 0���� Ŀ�� �ٸ��Ŷ� �ٲ� �� ����
                        if(row > 0)
                        {
                            //���� �ٲ� ���� �ప�� ����

                            //Ŭ���� ������Ʈ�� �������� �����̸� row ���� �ϳ� �پ��� ��
                            _GameBoard[column, row].GetComponent<Block>().Row = row - 1;
                            //Ŭ���� ���� ���ʿ� �ִ� �ִ� ���������� �������� ��
                            _GameBoard[column,row-1].GetComponent<Block>().Row = row;

                            //���Ӻ������ ���� �������� �ٲ۴�
                            //=> 
                            _GameBoard[column,row] = _GameBoard[column,row-1];
                            _GameBoard[column, row-1] = _ClickObject;

                            //���� �¿������� �����δ�
                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.RIGHT);
                            _GameBoard[column, row - 1].GetComponent<Block>().Move(DIRECTION.LEFT);
                        }
                    }
                break;
                case MouseMoveDirection.MOUSEMOVERIGHT:
                    {
                        //���� Ŭ���� ���� ��� �� ���� �� ������Ʈ���� ������ �´�
                        int column = _ClickObject.GetComponent<Block>().Column;
                        int row = _ClickObject.GetComponent<Block>().Row;
                        //���� �ٲ� �� ���� �ȿ����� �ٲ��� �Ѵ�

                        //������ �������� �۾ƾ� �ϱ� ������ ������ ����
                        if (row < (_Row-1))
                        {
                            //���� �ٲ� ���� �ప�� ����

                            //Ŭ���� ������Ʈ�� �������� �����̸� row ���� �ϳ� �پ��� ��
                            _GameBoard[column, row].GetComponent<Block>().Row = row + 1;
                            //Ŭ���� ���� ���ʿ� �ִ� �ִ� ���������� �������� ��
                            _GameBoard[column, row + 1].GetComponent<Block>().Row = row;

                            //���Ӻ������ ���� �������� �ٲ۴�
                            //=> 
                            _GameBoard[column, row] = _GameBoard[column, row +1];
                            _GameBoard[column, row +1] = _ClickObject;

                            //���� �¿������� �����δ�
                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.LEFT);
                            _GameBoard[column, row + 1].GetComponent<Block>().Move(DIRECTION.RIGHT);
                        }
                    }
                    break;
                case MouseMoveDirection.MOUSEMOVEUP:
                    {
                        //���� Ŭ���� ���� ��� �� ���� �� ������Ʈ���� ������ �´�
                        int column = _ClickObject.GetComponent<Block>().Column;
                        int row = _ClickObject.GetComponent<Block>().Row;
                        //���� �ٲ� �� ���� �ȿ����� �ٲ��� �Ѵ�

                        //������ 0���� Ŀ�� �ٸ��Ŷ� �ٲ� �� ����
                        if ( column > 0)
                        {
                            //���� �ٲ� ���� ������ ����

                            //Ŭ���� ������Ʈ�� ���� �����̸� col ���� �ϳ� �پ��� ��
                            _GameBoard[column, row].GetComponent<Block>().Column = column - 1;
                            //Ŭ���� ���� �Ʒ��ʿ� �ִ� �ִ� ���� �������� ��
                            _GameBoard[column - 1, row].GetComponent<Block>().Column = column;

                            //���Ӻ������ ���� �������� �ٲ۴�
                            //=> 
                            _GameBoard[column, row] = _GameBoard[column - 1, row];
                            _GameBoard[column - 1, row] = _ClickObject;

                            //���� �������� �����δ�
                            _GameBoard[column - 1, row].GetComponent<Block>().Move(DIRECTION.UP);
                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.DOWN);
                        }
                    }
                    break;
                case MouseMoveDirection.MOUSEMOVEDOWN:
                    {
                        //���� Ŭ���� ���� ��� �� ���� �� ������Ʈ���� ������ �´�
                        int column = _ClickObject.GetComponent<Block>().Column;
                        int row = _ClickObject.GetComponent<Block>().Row;
                        //���� �ٲ� �� ���� �ȿ����� �ٲ��� �Ѵ�

                        //������ ��ü ������ �۾ƾ� �ٸ��Ŷ� �ٲ� �� ����
                        if (column < _Column-1)
                        {
                            //���� �ٲ� ���� ������ ����

                            //Ŭ���� ������Ʈ�� �Ʒ����� �����̸� �� ���� �ϳ� �þ�� ��
                            _GameBoard[column, row].GetComponent<Block>().Column = column + 1;
                            //Ŭ���� ���� ���� �ִ� �ִ� �Ʒ��� �������� ��
                            _GameBoard[column+1, row].GetComponent<Block>().Column = column;

                            //���Ӻ������ ���� �������� �ٲ۴�
                            //=> 
                            _GameBoard[column, row] = _GameBoard[column + 1, row];
                            _GameBoard[column + 1, row] = _ClickObject;

                            //���� ���������� �����δ�
                            _GameBoard[column + 1, row].GetComponent<Block>().Move(DIRECTION.DOWN);
                            _GameBoard[column, row].GetComponent<Block>().Move(DIRECTION.UP);
                        }
                    }
                    break;

            }

        }
    }
    /// <summary>
    /// ���콺 �̵� �� �̵��� ������ ����ϴ� �Լ�
    /// </summary>
    /// <param name="mouseMove"></param>
    /// <returns></returns>
    private MouseMoveDirection CalculateDirection ()
    {
        float angle = CalculateAngle(_StartPos, _EndPos);   //z���� �������� ȸ���ϴ� �������� ���Ѵ�
        /*
         up���͸� 0���� ���� ��, �ð�������� 0 45 90 135 180 225 270 315 360 
         */
        if (angle >= 315.0f && angle <= 360 || angle >= 0 && angle < 45.0f)
        {
            return MouseMoveDirection.MOUSEMOVEUP;
        }
        else if(angle >= 45f && angle< 135f)
        {
            //ȭ�� �ݴ����� ���� ������ left�� �ٲ��
            return MouseMoveDirection.MOUSEMOVELEFT;
        }
        else if(angle >= 135f && angle < 225f)
        {
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
        else if(angle >= 225f && angle < 315f)
        {
            //ȭ�� �ݴ����� ���� ������ left�� �ٲ��
            return MouseMoveDirection.MOUSEMOVERIGHT;
        }
        else
        {
            //������ ������ �ɸ��Ŷ� else �ǹ̴� ���� ( �׳� right �� else �ؼ� �����ص� ��
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
    }
    /// <summary>
    /// �� ���� ������ ������ ���ϴ� �Լ�
    /// </summary>
    /// <param name="from"> => Ŭ���� �� </param>
    /// <param name="to">=> Ŭ���ϰ� �� ��</param>
    /// <returns></returns>
    private float CalculateAngle(Vector3 from, Vector3 to)
    {
        //���� �� �κ� ������ ���� �� �ٿ������ ���� ��ġ�� ���� ������ ���Ѵ�
        //to ���� from ��ǥ���� ���� from ���� to�� ���ϴ� ��ǥ���� ���� �� �ִ�
        //up���Ϳ� to ���� ������ ������ ���ϴ� �Լ��� FromToRotation�̴�.
        //�츮�� �Ĵٺ��� ��ǥ�� z �̱� ������, ȸ�� ���� �߿����� z ���� ���Ѵ� => z���� �������� �� ��Ÿ��
        ////�츮�� �޸��� �������� �Ĵٺ��� ���� (��������)
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }
    //�Է��� ��� �Դ� ���� �����Ѵ�.
    private bool _InputOK;  
    /// <summary>
    /// ���� �������� �������� üũ�ϴ� �Լ�
    /// �����̴� ���� �ִ��� Ȯ���Ѵ�
    /// </summary>
    /// <returns></returns>
    /// 
    private bool CheckBlockMove()
    {
        foreach (var item in _GameBoard)
        {
            if(item !=null)
            {
                if(item.GetComponent<Block>().State == BLOCKSTATE.MOVE)
                {
                    return false;
                }
            }
        }
        return true;
    }
    //������ �� ����
    private List<GameObject> _RemovingBlocks = new List<GameObject>();
    //������ �� ����
    private List<GameObject> _RemovedBlocks = new List<GameObject>();
    /// <summary>
    /// 3��Ī�� ���� ã�� �Լ�
    /// 3��ġ�Ǹ� ���� ������� ���� �������� �� �ڸ��� �����.
    /// �� �� �ڸ��� ���ο� ���� ä�� ���� �� ȭ�鿡 �Ⱥ��̰Ը� �ϰ� ������ �Ѵ�.
    /// �������� �� �ְ� �� ������ �� �ִ�. �� ó�� Ÿ���� üũ�ϰ� �� ���� ���� ����ϰ� �� ���� ���� üũ�ϴ� ����
    /// </summary>
    private void CheckMatchBlock()
    {
        //3�� �̻� ���� ���� �ִ��� üũ�Ѵ�
        //��Ī�� ���� ���� �� ����Ʈ 
        List<GameObject> matchList = new List<GameObject>();
        // �ӽ÷� ��Ī�� ���� ����
        List<GameObject> tempMatchList = new List<GameObject>();
        //���� ��ġ ���� Ÿ���� ������ ���� (���� �� Ÿ���� ���)
        int checkType = 0;
        //������ �� ����� ����Ʈ �ʱ�ȸ
        _RemovedBlocks.Clear();
        //���ι������� ���� ���� �ִ��� üũ
        for (int row = 0; row < _Row; row++)
        {
            if (_GameBoard[row,0]==null)
            {
                continue;
            }
            //ù ���� ���� Ÿ�԰��� �����´�
            checkType = _GameBoard[row, 0].GetComponent<Block>().Type;
            //ó�� ���� �ӽ� ��ġ�� ��������� �߰��Ѵ�
            tempMatchList.Add(_GameBoard[row, 0]);

            for (int col = 1; col < _Column; col++)
            {
                //���� ������ ��Ƽ��
                if (_GameBoard[row,col]==null)
                {
                    continue;
                }

                if(checkType == _GameBoard[row,col].GetComponent<Block>().Type) 
                {
                    tempMatchList.Add(_GameBoard[row, col]);
                }
                else
                {
                    //��ġ�Ǵ� ���� 3�� �̻��̸�?
                    if(tempMatchList.Count>=3)
                    {
                        //��Ī�� �� ����Ʈ�� �ӽ� ��ġ ����Ʈ�� �ִ°� �ű��
                        matchList.AddRange(tempMatchList);
                        //���� �ű�� ���� �ƴ϶� ���� �������� ���ش� (�ٽ� üũ�ؾ� �ϱ� ����)
                        tempMatchList.Clear();
                        //����ġ�� �� ��ġ�� �� Ÿ�� ���� �ٽ� ����
                        checkType = _GameBoard[row,col].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[row, col]);
                    }
                    else
                    {
                        //����ġ�� �� ��ġ���� ������ ���� 3���� ��ġ���� �ʾ���=>�ٽ� �ʱ�ȭ
                        tempMatchList.Clear();
                        //���� �ٽ� �����Ѵ�
                        checkType = _GameBoard[row, col].GetComponent<Block>().Type;
                        tempMatchList.Add(_GameBoard[row, col]);
                    }
                }
            }
            //���� �� ������
            //���� ���� ���� �� �� �� �ٽ� �ѹ� üũ�Ѵ�
            //��������Ʈ�� ���� count�� üũ�Ѵ�
            if(tempMatchList.Count>=3)
            {
                matchList.AddRange(tempMatchList);
                tempMatchList.Clear();
            }
            else
            {
                //������ ��ġ���� ���� 
                tempMatchList.Clear();
            }
        }
        tempMatchList.Clear();
        //=================================================
        //���� �������� ��ġ ���� üũ�Ѵ�
        for (int col = 0; col < _Column; col++)
        {
            if (_GameBoard[col,0]==null)
            {
                continue;
            }
            //ù ���� �� Ÿ�԰��� �����Ѵ�
            checkType = _GameBoard[0,col].GetComponent<Block>().Type;
            tempMatchList.Add(_GameBoard[0,col]);

            for (int row = 0; row < _Row; row++)
            {
                if (_GameBoard[row,col] == null)
                {
                    continue;
                }

                if(checkType == _GameBoard[row,col].GetComponent<Block>().Type)
                {
                    tempMatchList.Add(_GameBoard[row, col]);
                }
                else
                {
                    //����ġ ( �ش� ��ġ�� ���� Ÿ���� Ʋ��
                    if(tempMatchList.Count>= 3) 
                    { 
                        //������ �ִ°� ��ġ�� �ű��
                        matchList.AddRange(tempMatchList);
                        //Ŭ����
                        tempMatchList.Clear();
                         //����ġ�� ��Ÿ�� ��ġ���� �ٽ� ���� Ÿ���� �����´� ( �� ��ġ��������)
                        checkType = _GameBoard[row, col].GetComponent<Block>().Type;
                        //����ġ�� ��Ÿ�� ���� ��ġ�� �ٽ� ����Ѵ�
                        tempMatchList.Add(_GameBoard[row, col]);
                    }
                    else
                    {
                        //���� ��Ī ������ 3���� ���� ���
                        tempMatchList.Clear();
                        //�� ��ġ���� �ٽ� üũŸ�� �����´� ( �ٽ� 3���� ��ġ�ϴ��� ��������)
                        checkType = _GameBoard[row, col].GetComponent<Block>().Type;
                        //���� �߰��Ѵ�
                        tempMatchList.Add(_GameBoard[row, col]);
                    }
                }
            }
            //���� �� �� �� 3�� �̻��� ��Ī�Ȱ��

            if(tempMatchList.Count>=3)
            {
                matchList.AddRange(tempMatchList);
                tempMatchList.Clear();
            }
            else
            {
                tempMatchList.Clear();
            }
        }
        //��ġ����Ʈ�� �ߺ��� ���� �߷�����
        //�ߺ��Ȱ� ���� �����ش�

        matchList = matchList.Distinct().ToList();

        if(matchList.Count>=0) 
        {
            foreach (var item in matchList)
            {
                //���ӿ�����Ʈ�� ���� (item)
                //��ġ�� ���� ��Ȱ��ȭ ����.
                //�ı��ϰ� �ٽ� ����°��� ����� ���� ��� ������ �ǵ����̸� ��Ȱ��ȭ �ؾ� �Ѵ�
                //��ġ�� ���� ����󿡼� �����ؾ��ϱ� ������ ���� ����󿡼� ���ֹ�����
                //null�� �־ ���ֹ���.
                //�� ���� ��ġ�� �Ǹ� �� ��ü�� �������� ������ �� �κ��� �� ó���� �Ѵ� ( ����󿡼�)
                //���Ӻ��� �󿡼� �ش� ��ġ�l ���� ��ó�� 
                _GameBoard[item.GetComponent<Block>().Column, item.GetComponent<Block>().Row]=null;
                //���� �Ⱥ��̰� ó��
                item.SetActive(false);
            }

            //��ġ����Ʈ�� �ִ� ���� removing �� (������ ������ ����)�� �ִ´�
            _RemovingBlocks.AddRange(matchList);
        }
        //������ ������ ���� ���� �� ����Ʈ�� �����Ѵ�
        _RemovedBlocks.AddRange(_RemovingBlocks);
        //�ߺ��� �ȵǰ� ó�� ( �ߺ��� ���� ó���ϴ� �Լ�)
        _RemovedBlocks = _RemovedBlocks.Distinct().ToList();

        //�ߺ��� �� üũ�� ������ ��ĭ�� ä��� ���� ���� �ϰ���Ų��
        DownMoveBlocks();
    }

    private void DownMoveBlocks()
    {
        //�������� �� ������ ī�����ϴ� ���� 
        //�� ĭ�� �ϰ��ؾ� �ϴ���
        int moveCount = 0;

        //���ι������� ���ƾ� �ϴϱ� �ٱ��� row ������ col �� �Ѵ�
        for (int row =0; row<_Row; row++)
        {
            for (int col = _Column-1; col >= 0; col--)
            {
                //�ؿ����⿡������ �������� ��
                //�Ʒ����� 4-3-2-1-0 �̷��� ���ƾ� �Ѵ�
                //���Ӻ���� ���� ���� ��쿡 �� ó���� �߾���
                if (_GameBoard[row,col]==null)
                {
                    moveCount++;
                }
                else
                {
                    //���Ӻ���� ���� �ִ� ���
                    if(moveCount>0)
                    {
                        //�ϰ������� üũ�Ѵ�
                        Block block = _GameBoard[row,col].GetComponent<Block>();
                        //���� ��ġ���� ���
                        block.MovePos = block.transform.position;
                        //x���� �׳� ���θ� ��, 
                        //y ���� moveCount * ���� ���̸�ŭ �̵��� �Ÿ��� �����ش�
                        //�̵��� ��ġ���� ����Ѵ� ( �󸶸�ŭ �̵��Ұ���)
                        block.MovePos = new Vector3(block.MovePos.x, block.MovePos.y - block.Width * moveCount, block.MovePos.z);
                        //������ �ִ� ���Ӻ������ ��ġ�� �ʱ�ȭ
                        _GameBoard[row, col] = null;
                        //�̵��� ���� �÷����� �ο찪�� ����ؼ� �־��ش�
                        //������ ��ġ���� moveCount ��ŭ �������� ������ ��������
                        //���Ӻ������ �̵��� ��ġ�� �����Ѵ�
                        block.Column = block.Column + moveCount;
                        //���� �̸��� �����Ѵ�
                        block.gameObject.name = $"Block[{block.Row},{block.Column}]";
                        //���Ӻ������ �̵��� ��ġ�� ���� �������� ����
                        _GameBoard[block.Column, block.Row] = block.gameObject;

                        //���� �������� ó���ϴ� �Լ� (������ ������)
                        block.Move(DIRECTION.DOWN, moveCount);
                    }
                }
            }
            //�� ���� ������ ������ �ʱ�ȭ���ش�
            moveCount= 0;
        }

    }

    void Update()
    {
        /*
         ���� : ���� ���η����� ���ư��� -> �Է� Ȯ���� �ϰ� -> �Է¿� ���� ���μ��� ó���ϰ� -> ȭ�鿡 ������ �Ѵ�
         */

        //���콺 ��ư�� ������ �� + �Է��� ������ ������ ��
        if(Input.GetMouseButtonDown(0) && _InputOK == true)
        {
            //���콺 Ŭ���� ���·� ����
            _MouseClick = true;
            _IsOver = false;

            //Bounds �������� ��� �̰ɷ� ���� 
            Vector3 pos = Input.mousePosition;
            //pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));

            //StartPos�� EndPos �� ���콺�� ó�� Ŭ���� ��ǥ�� ����
             _EndPos =_StartPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));   //Ŭ���� �Ǿ��� �� ��ġ�� �������� ���� ��ġ������ �ʱ�ȭ
            //z���� ���� ������ 0���� �ʱ�ȭ
            _EndPos.z = _StartPos.z = 0f;

            //2D������ ���콺�� �ϴ°� �� ���� (3D�� ������ �߻��ؼ� ���-> ���ſ�)

            //���콺 ��ư�� ������ ��, ���� ��ġ���� �����´�
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Pos " + pos);

            //��ũ���󿡼� ���� ������ ���� ��µȴ�.
            //�׷��� �� ��ǥ�� � ���� �ִ��� Ȯ���ؾ��� 
            //���� �簢���� �ȿ� �츮�� ���� ���� ���ԵǾ��ִ��� ���� ������ŭ ������ �Ѵ�
            //���Ӻ��忡 ���� �־���� ������ �ű� �ִ� ����ŭ ������ �Ǳ� ������ col�� row ���� ����ϴ� ����ʵ带 �����

            //Ŭ���� �Ǿ����� �� �Ǿ����� üũ�ϴ� ����

            for (int i = 0; i < _Column; i++)
            {
                for (int j   = 0; j < _Row; j++)
                {
                    //Ŭ���� ��ǥ���� �������� ���� ã�� ��
                    //���� ���� �ִ����� üũ�Ѵ� (���� ��쵵 ����)

                    if (_GameBoard[i,j] != null)
                    {
                        //Ŭ���� �Ǿ����� �� �Ǿ����� üũ�ϴ� ����
                        //������ ������ �� ������Ʈ�� �θ�������Ʈ�� �ִ� ���� �����ϰ� �װ� �� �ڽĵ��� spriterenderer�� �����ؼ�
                        //�簢���� bounds �� �����´�. bound�ȿ� a���콺 Ŭ���� ���� ��ǥ�� ��ġ�� ���ԵǴ��� ���θ� bool������ �����Ѵ�
                        //���ԵǾ������� true �� ���ϵǾ �Ʒ��� �̵��Ѵ�
                        _IsOver = _GameBoard[i,j].GetComponent<Block>().BlockImage.GetComponent<SpriteRenderer>().bounds.Contains(_StartPos);
                        

                        //���࿡ ������Ʈ�� ������� �Ϸ��� �ݶ��̴��� �־��ش�.
                    }
                    if(_IsOver == true)
                    {
                        //���� ���� �̸��� ��µȴ�.
                        Debug.Log("ClickObj = "+_GameBoard[i,j].name);

                        //Ŭ���� ������Ʈ�� �������� Ŭ��������Ʈ ����ʵ忡 �Ҵ�
                        _ClickObject = _GameBoard[i, j];

                        //��ø�Ǿ��ִ� for���� �ѹ��� ���������� (�׷��� ������ break �� �� �� ����� ��)
                        goto LoopExit;
                    }
                }
            }
            LoopExit:;
        }
        //���콺 ��ư�� ���� �� => �Է��� ������ ���·� ����
        if (Input.GetMouseButtonUp(0))
        {
            _ClickObject = null;
            _MouseClick = false;
            _InputOK= true;
        }
        //���콺�� Ŭ���� ���¿��� ���콺�� � �����̵� ��ȭ�� �ִ���
        //1. ���콺 Ŭ���� TRUE �� ó�� (Ŭ���� ���¿��� �巡�׸� �ߴ��� ���ߴ��� ó��)
        //2. X������ �̵��ߴ��� (��ȭ���� �ִ��� => ������̵� ��� ����. 0�̸� ��ȭ���� ����)
        if (_MouseClick == true && (Input.GetAxis("Mouse X") < 0) || (Input.GetAxis("Mouse X") > 0)
            || (Input.GetAxis("Mouse Y") < 0) || (Input.GetAxis("Mouse Y") > 0))
        {
            //��ȭ�� �ִ� ���콺 Ŀ���� ��ġ�� �����ͼ� �� ENDPOS�� ����
            //Ŭ���� ���¿��� ���콺�� ������ ��. �� ������ ��ġ���� endPos�� �ȴ�
            //Ŭ������ startPos�� �ǰ� ���������̶� �����̸� �װ� endpos�� ���Ѵ�
            Vector3 pos = Input.mousePosition;
            _EndPos = Camera.main.ScreenToWorldPoint(new Vector3( pos.x, pos.y, 10.0f));
            _EndPos.z = 0f;

            //���콺�� ���������� ó���ϴ� �Լ�
            MouseMove();
            CheckMatchBlock();
        }

       
        
    }
}
