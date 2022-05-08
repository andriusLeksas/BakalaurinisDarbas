using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerTests
{

   [Test]
   public void PlayerObjectWasCreatedTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        Assert.NotNull(player);
    }

    [Test]
    public void AllRespawnObjectsExistTest()
    {
        var respawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int count = 6;

        Assert.AreEqual(respawnPoints.Length, count);
    }

    [Test]
    public void TakeHitPlayerFunctionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var script = player.GetComponent<FPSController>();
        script.health = 100;

        script.TakeHit(20);

        Assert.AreEqual(script.health, 80);
        
    }

    //[Test]
    //public void OnTriggerEnterVaccineTest()
    //{
    //    var player = GameObject.FindGameObjectWithTag("Player");
    //    var playerScript = player.GetComponent<FPSController>();

    //    var gun = GameObject.FindGameObjectWithTag("Player");

    //    var vaccine = GameObject.FindGameObjectWithTag("Vacinee");

    //    playerScript.OnTriggerEnter(vaccine.GetComponent<Collider>());

    //    Assert.NotNull(playerScript.AbillityArray[0]);

    //}

    [Test]
    public void playStepsFucntionTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerScript = player.GetComponent<FPSController>();

        playerScript.PlaySteps();

        Assert.IsTrue(playerScript.playingWalking);

    }

    [Test]
    public void gunOnEnableTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");      
        var gun = GameObject.Find("SciFiGunLightWhite");
        var gunScript = gun.GetComponent<Gun>();
        gunScript.Start();
        gunScript.OnEnable();

        Assert.IsFalse(gunScript.isRealoading);
    }

    [Test]
    public void isReloadingTest()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var gun = GameObject.Find("SciFiGunLightWhite");
        var gunScript = gun.GetComponent<Gun>();

        gunScript.Start();
        gunScript.OnEnable();
        gunScript.isRealoading = true;
        gunScript.Update();

        Assert.IsTrue(gunScript.isRealoading);
    }

    [Test]
    public void currentAmmoIsLessThan0Test()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var gun = GameObject.Find("SciFiGunLightWhite");
        var gunScript = gun.GetComponent<Gun>();

        gunScript.Start();
        gunScript.OnEnable();
        gunScript.currentAmmo = 0;
        gunScript.Update();     
        Assert.AreEqual(gunScript.currentAmmo, 30);
    }

}
