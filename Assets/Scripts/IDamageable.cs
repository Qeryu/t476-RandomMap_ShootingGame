using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//takenDamage接口。接口首字母一般用大写的I表示,从抽象类（未完全实现逻辑的类）演化而来的完全未实现
//她是允许任何脚本去访问的所以我们不能添加任何限制性级别,不用去写public
public interface IDamageable
{
    void TakenDamage(float _damageAmount);
    //z这里是不需要在接口去写方法体的，因为这个方法本身就是等待实现的脚本去实现的，
    //任何使用这个接口的脚本都必须在他自己中实现这个方法
    void TakenHit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    void ChangeSpeed();
    void RecoverSpeed();
}
