using System;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Ship _ship;
    [SerializeField] private PoolBullets _bullets;
    [SerializeField] private CameraInfo _camera;
    [SerializeField] private float _offsetFromBorder;

#pragma warning restore 0649
    #endregion

    public event Action OnDead;

    private float _rechargeDelay;
    private Vector3 _positionStart;

    public bool IsCanShoot => _rechargeDelay == 0;

    public void Reset()
    {
        _ship.Position = _positionStart;
    }

    private void OnEnable()
    {
        _positionStart = _ship.Position;
    }

    private void Update()
    {
        RechargeProcessing();

        if (ShootProcessing())
        {
            ShootRecharge();
        }

        MoveProcessing();
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        Dead();
    }

    private void RechargeProcessing()
    {
        if (_rechargeDelay > 0)
        {
            _rechargeDelay -= Time.deltaTime;
        }
        if (_rechargeDelay < 0)
        {
            _rechargeDelay = 0;
        }
    }

    private void ShootRecharge() => _rechargeDelay = _ship.ShootRechargeTime;
    private void Shoot() => _bullets.Get().Shoot(_ship.PivotBulletPosition, _ship.PivotBulletDirection, _ship.SpeedBullet);

    private bool ShootProcessing()
    {
        if (IsCanShoot && Input.GetKey(KeyCode.Space))
        {
            Shoot();
            return true;
        }

        return false;
    }

    private void MoveProcessing()
    {
        var left = _camera.Left + _offsetFromBorder;
        var rigth = _camera.Rigth - _offsetFromBorder;

        var position = _ship.Position;

        position += CollectMove();
        position.x = Mathf.Clamp(position.x, left, rigth);

        _ship.Position = position;
    }

    private Vector3 CollectMove()
    {
        var deltaPosition = Vector3.zero;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            deltaPosition += GetSpeedFromDirection(-transform.right);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            deltaPosition += GetSpeedFromDirection(transform.right);
        }
        return deltaPosition;
    }

    private Vector3 GetSpeedFromDirection(Vector3 dir) => dir * (_ship.Speed * Time.deltaTime);

    private void Dead() => OnDead?.Invoke();
}
