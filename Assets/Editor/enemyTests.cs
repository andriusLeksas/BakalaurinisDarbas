using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyTests 
{
    public enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD };

    [Test]
    public void TurnOffTriggersIsWalkingCheckTest()
    {
        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Start();
        zombieScript.TurnOffTriggers();

        Assert.IsFalse(zombieScript.anim.GetBool("isWalking"));
    }

    [Test]
    public void TurnOffTriggersIsDeadCheckTest()
    {
        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Start();
        zombieScript.TurnOffTriggers();

        Assert.IsFalse(zombieScript.anim.GetBool("isDead"));
    }

    [Test]
    public void TurnOffTriggersIsAttackingCheckTest()
    {
        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Start();
        zombieScript.TurnOffTriggers();

        Assert.IsFalse(zombieScript.anim.GetBool("isAttacking"));
    }

    [Test]
    public void TurnOffTriggersIsRunningCheckTest()
    {
        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Start();
        zombieScript.TurnOffTriggers();

        Assert.IsFalse(zombieScript.anim.GetBool("isRunning"));
    }

    [Test]
    public void PlayAttackAudioTest()
    {
        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.PlayAttackAudio();

        Assert.IsTrue(zombieScript.attacks[0].isPlaying);
    }

    [Test]
    public void damagePlayerFunctionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var script = player.GetComponent<FPSController>();
        script.health = 100;

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();
        zombieScript.target = player;
        zombieScript.damageAmount = 25;

        zombieScript.DamagePlayer();

        Assert.AreEqual(script.health, 75);

    }

    [Test]
    public void distanceToPlayerFunctionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var script = player.GetComponent<FPSController>();

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();
        zombieScript.target = player;
        zombieScript.damageAmount = 25;

        var distance = zombieScript.DistanceToPlayer();

        Assert.AreNotEqual(distance, Mathf.Infinity);

    }

    [Test]
    public void canSeePlayerFunctionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var script = player.GetComponent<FPSController>();

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();
        zombieScript.target = player;

        bool canSeePlayer = zombieScript.CanSeePlayer();

        Assert.IsFalse(canSeePlayer);

    }

    [Test]
    public void forgetPlayerFunctionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var script = player.GetComponent<FPSController>();

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();
        zombieScript.target = player;

        bool canSeePlayer = zombieScript.ForgetPlayer();

        Assert.IsTrue(canSeePlayer);

    }

    [Test]
    public void TakeDamageFunctionTest()
    {

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.health = 30f;

        zombieScript.TakeDamage(10f);

        Assert.AreEqual(zombieScript.health, 20f);

    }

    [Test]
    public void killZombieFunctionTest()
    {

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Start();
        zombieScript.KillZombie();

        Assert.IsTrue(zombieScript.anim.GetBool("isDead"));

    }

    [Test]
    public void idleStateTest()
    {

        var zombie = GameObject.FindGameObjectWithTag("Zombie");
        var zombieScript = zombie.GetComponent<ZombieController>();

        zombieScript.Update();

        Assert.AreEqual(zombieScript.state, ZombieController.STATE.IDLE);

    }

    [Test]
    public void bossGetAnimatorOnStartTest()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();

        bossScript.Start();

        Assert.IsNotNull(bossScript.ani);

    }

    [Test]
    public void bossGetTargetOnStartTest()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();

        bossScript.Start();

        Assert.IsNotNull(bossScript.target);

    }

    [Test]
    public void bossTakeDamageTest()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();

        bossScript.Start();
        bossScript.Hp_Min = 500;
        bossScript.TakeDamage(50);

        Assert.AreEqual(bossScript.Hp_Min, 450);

    }

    //[Test]
    //public void bossDyingAnimationTest()
    //{

    //    var boss = GameObject.FindGameObjectWithTag("Boss");
    //    var bossScript = boss.GetComponent<Boss>();

    //    bossScript.Start();
    //    bossScript.Hp_Min = 40;
    //    bossScript.TakeDamage(50);

    //    Assert.IsTrue(bossScript.death);

    //}

    [Test]
    public void bossRoutine0Test()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();

        bossScript.Start();
        bossScript.visionDistance = 1000000;
        bossScript.routine = 0;
        bossScript.atacking = false;
        bossScript.BossBehaviour();

        Assert.IsTrue(bossScript.ani.GetBool("walk"));

    }

    [Test]
    public void bossRoutine1Test()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();

        bossScript.Start();
        bossScript.visionDistance = 1000000;
        bossScript.routine = 1;
        bossScript.atacking = false;
        bossScript.BossBehaviour();

        Assert.IsTrue(bossScript.ani.GetBool("run"));

    }

    [Test]
    public void bossRoutine2Test()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();
        var range = GameObject.Find("Range");
        bossScript.range = range.GetComponent<BossSkills>();

        bossScript.Start();
        bossScript.visionDistance = 1000000;
        bossScript.routine = 2;
        bossScript.atacking = false;
        bossScript.faze = 3;
        bossScript.BossBehaviour();

        Assert.AreEqual(0.8f, bossScript.ani.GetFloat("skills"));

    }

    [Test]
    public void bossRoutine3Test()
    {

        var boss = GameObject.FindGameObjectWithTag("Boss");
        var bossScript = boss.GetComponent<Boss>();
        var range = GameObject.Find("Range");
        bossScript.range = range.GetComponent<BossSkills>();

        bossScript.Start();
        bossScript.visionDistance = 1000000;
        bossScript.routine = 3;
        bossScript.direction_skill = true;
        bossScript.atacking = false;
        bossScript.faze = 3;
        bossScript.BossBehaviour();

        Assert.AreEqual(0.6f, bossScript.ani.GetFloat("skills"));

    }

}
