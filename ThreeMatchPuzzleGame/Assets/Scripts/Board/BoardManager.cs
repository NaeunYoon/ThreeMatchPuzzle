using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using Unity.Mathematics;
using UnityEngine;


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

    private Vector3 _screenPos; //��ũ�� 0,0���� ���� ��ǥ�� ����
    float _ScreenWidth; //��ũ�� ����
    float _BlockWidth;  //�� �ϳ��� ���̸� �����ϱ� ���� ����
    float _ScreenHeight;    //��ũ�� ����
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




        MakeBoard(10,10);
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
        float width = _ScreenWidth -(_Xmargin * 2); //��µǴ� �ʺ� ( 5.6�̵�. ��ũ�� x �� 2.8�̰� ��ü ��ũ�� �ʺ� ���� ��)
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
        Debug.Log("MouseMove");
        //����üũ
        //�� ���ͻ����� �Ÿ��� ���Ѵ�
        float diff = Vector2.Distance(_StartPos, _EndPos);
        //���� 
        //������ �Ÿ��� ���� ������ MoveDistance ���� Ŭ �� ó��, MoveDistance : ���� 0.01f
        // Ŭ���� ������Ʈ�� ���� ��
        if(diff > _MoveDistance && _ClickObject != null) 
        {
            //����� �������� �������� �Ͼ���� ���
            // ���콺 ���� enum, ���콺 �̵� �� ������ ����ϴ� �Լ�, ���� ��� �ڿ� �� ������ ������ ���ϴ� �Լ�
            MouseMoveDirection dir = CalculateDirection();
            Debug.Log("Direction "+dir);
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
        if (angle > 315.0f && angle <= 360 || angle >= 0 && angle < 45.0f)
        {
            return MouseMoveDirection.MOUSEMOVEUP;
        }
        else if(angle >= 45f && angle<135f)
        {
            //ȭ�� �ݴ����� ���� ������ left�� �ٲ��
            return MouseMoveDirection.MOUSEMOVELEFT;
        }
        else if(angle >= 135f && angle <225f)
        {
            return MouseMoveDirection.MOUSEMOVEDOWN;
        }
        else if(angle>=225f && angle<315f)
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
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private float CalculateAngle(Vector3 from, Vector3 to)
    {
        //���� �� �κ� ������ ���� �� �ٿ������ ���� ��ġ�� ���� ������ ���Ѵ�
        //to ���� from ��ǥ���� ���� from ���� to�� ���ϴ� ��ǥ���� ���� �� �ִ�
        //up���Ϳ� to ���� ������ ������ ���ϴ� �Լ��� FromToRotation�̴�.
        //�츮�� �Ĵٺ��� ��ǥ�� z �̱� ������, ȸ�� ���� �߿����� z ���� ���Ѵ� => 
        return Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;
    }

    void Update()
    {
        /*
         ���� : ���� ���η����� ���ư��� -> �Է� Ȯ���� �ϰ� -> �Է¿� ���� ���μ��� ó���ϰ� -> ȭ�鿡 ������ �Ѵ�
         */

        //���콺 ��ư�� ������ �� 
        if(Input.GetMouseButtonDown(0))
        {
            //���콺 Ŭ���� ���·� ����
            _MouseClick = true;

            //Bounds �������� ��� �̰ɷ� ���� 
            Vector3 pos = Input.mousePosition;
            //pos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));

            //StartPos�� EndPos �� ���콺�� ó�� Ŭ���� ��ǥ�� ����
             _EndPos =_StartPos = Camera.main.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 10f));   //Ŭ���� �Ǿ��� �� ��ġ�� �������� ���� ��ġ������ �ʱ�ȭ
            //z���� ���� ������ 0���� �ʱ�ȭ
            _EndPos.z = _StartPos.x = 0f;


            //2D������ ���콺�� �ϴ°� �� ���� (3D�� ������ �߻��ؼ� ���-> ���ſ�)

            //���콺 ��ư�� ������ ��, ���� ��ġ���� �����´�
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Debug.Log("Mouse Pos " + pos);

            //��ũ���󿡼� ���� ������ ���� ��µȴ�.
            //�׷��� �� ��ǥ�� � ���� �ִ��� Ȯ���ؾ��� 
            //���� �簢���� �ȿ� �츮�� ���� ���� ���ԵǾ��ִ��� ���� ������ŭ ������ �Ѵ�
            //���Ӻ��忡 ���� �־���� ������ �ű� �ִ� ����ŭ ������ �Ǳ� ������ col�� row ���� ����ϴ� ����ʵ带 �����

            //Ŭ���� �Ǿ����� �� �Ǿ����� üũ�ϴ� ����


             _IsOver = false;
            for (int i = 0; i < _Column; i++)
            {
                for (int j   = 0; j < _Row; j++)
                {
                    //Ŭ���� ��ǥ���� �������� ���� ã�� ��
                    //���� ���� �ִ����� üũ�Ѵ� (���� ��쵵 ����)


                    if (_GameBoard[i,j]!= null)
                    {
                        //Ŭ���� �Ǿ����� �� �Ǿ����� üũ�ϴ� ����
                        //������ ������ �� ������Ʈ�� �θ�������Ʈ�� �ִ� ���� �����ϰ� �װ� �� �ڽĵ��� spriterenderer�� �����ؼ�
                        //�簢���� bounds �� �����´�. bound�ȿ� a���콺 Ŭ���� ���� ��ǥ�� ��ġ�� ���ԵǴ��� ���θ� bool������ �����Ѵ�

                        _IsOver = _GameBoard[i,j].GetComponent<Block>().BlockImage.GetComponent<SpriteRenderer>().bounds.Contains(pos);
                        //isOver = true;

                        //���࿡ ������Ʈ�� ������� �Ϸ��� �ݶ��̴��� �־��ش�.


                    }

                    if(_IsOver==true)
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
        //���콺 ��ư�� ���� ��
        if(Input.GetMouseButtonUp(0))
        {
            _ClickObject= null;
            _MouseClick= false;

        }
        //���콺�� Ŭ���� ���¿��� ���콺�� � �����̵� ��ȭ�� �ִ���
        //1. ���콺 Ŭ���� TRUE �� ó�� (Ŭ���� ���¿��� �巡�׸� �ߴ��� ���ߴ��� ó��)
        //2. X������ �̵��ߴ��� (��ȭ���� �ִ��� => ������̵� ��� ����. 0�̸� ��ȭ���� ����)
        if(_MouseClick == true&&(Input.GetAxis("Mouse X")<0) || (Input.GetAxis("Mouse X")>0)
            || (Input.GetAxis("Mouse Y"))<0 || (Input.GetAxis("Mouse Y") > 0))
        {
            //��ȭ�� �ִ� ���콺 Ŀ���� ��ġ�� �����ͼ� �� ENDPOS�� ����
            //Ŭ���� ���¿��� ���콺�� ������ ��. �� ������ ��ġ���� endPos�� �ȴ�
            //Ŭ������ startPos�� �ǰ� ���������̶� �����̸� �װ� endpos�� ���Ѵ�
            Vector3 pos = Input.mousePosition;
            _EndPos = Camera.main.ScreenToWorldPoint(new Vector3( pos.x, pos.y, 10f));
            _EndPos.z = 0f;

            //���콺�� ���������� ó���ϴ� �Լ�
            MouseMove();
        }
    }
}
