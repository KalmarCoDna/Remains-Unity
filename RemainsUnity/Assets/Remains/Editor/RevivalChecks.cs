using System;
using Remains.Domain;
using Remains.Gameplay;
using UnityEngine;

namespace Remains.Editor
{
    public static class RevivalChecks
    {
        public static void Run()
        {
            SelfHealingReconciliationChecks.Run();
            CheckSelectedStack();
            CheckOcclusion();
            var healer=new ActorState {id="healer",player=true,connected=true};
            healer.inventory.Add(SharedInventory.Create("heal",2));
            var target=new ActorState {id="target",player=true,connected=true,down=true,health=0,x=1};
            var timer=new RevivalProgress();
            Require(!timer.Step(healer,target,true,.7f),"Неполное удержание не завершает лечение");
            timer.Step(healer,target,false,.1f);Require(timer.Progress==0,"Отпускание прерывает удержание");
            timer.Step(healer,target,true,.7f);healer.x=5;timer.Step(healer,target,true,.1f);
            Require(timer.Progress==0 && InventoryJournal.Count(healer,"heal")==2,"Отход не расходует предмет");healer.x=0;
            timer.Step(healer,target,true,.7f);healer.damageSequence++;healer.health=100;
            timer.Step(healer,target,true,.1f);Require(timer.Progress==0,"Урон прерывает лечение даже после восстановления HP");
            timer.Step(healer,target,true,.7f);target.generation++;timer.Step(healer,target,true,.1f);
            Require(timer.Progress==0,"Переподключение цели отменяет старое удержание");
            Require(timer.Step(healer,target,true,1.5f),"Непрерывное удержание завершается");
            Require(HealingRules.Apply(healer,target,false) && target.health==35 && !target.down && InventoryJournal.Count(healer,"heal")==1,"Успех расходует один предмет и поднимает с 35 HP");
            Require(!HealingRules.Apply(healer,target,false) && InventoryJournal.Count(healer,"heal")==1,"Повтор не расходует второй предмет");
            healer.inventory.Clear();target.down=true;target.health=0;
            Require(!timer.Step(healer,target,true,2) && timer.Progress==0,"Без предмета действие не запускается");
            var blast=new AttackEvent {attacker=healer.id,sourcePlayer=true,level=healer.level,weapon="grenade"};
            Require(AttackScope.CanAffect(blast,healer,false),"Собственный взрыв не исключает пострадавшего");
            Require(!AttackScope.CanAffect(blast,target,false) && AttackScope.CanAffect(blast,target,true),"Дружественный огонь управляет уроном союзникам");
            blast.weapon="pistol";Require(!AttackScope.CanAffect(blast,healer,true),"Прямой выстрел не попадает в автора");
            Debug.Log("RevivalChecks PASS: удержание, отпускание, расстояние, урон, поколение, однократный расход.");
        }
        private static void CheckOcclusion()
        {
            var world=new WorldState();
            var healer=new ActorState {id="healer",player=true,connected=true,level="hub",y=1};
            var target=new ActorState {id="target",player=true,connected=true,down=true,level="hub",x=1,y=1};
            healer.inventory.Add(SharedInventory.Create("heal",2));world.actors.Add(healer);world.actors.Add(target);
            var floor=new RoomSurface {x=-2,y=0,width=5,height=1};
            var wall=new RoomSurface {x=.45f,y=.5f,width=.1f,height=2};
            var room=new RoomLayout {platforms=new[]{floor,wall}};
            var focus=new InteractionFocus();var cursor=InventorySource.BodyBounds(target).center;
            var timer=new RevivalProgress();
            var rules=GameplayRuleSet.CreateDefault();
            void Aim()=>focus.Step(world,healer,rules,cursor,false,null,room:room);
            Aim();Require(focus.Target.kind!=InteractionKind.Revival,"Сплошная стена скрывает цель поднятия.");
            Require(!timer.Step(healer,target,true,2,room) && timer.Progress==0,"Стена блокирует начало удержания.");
            Require(!HealingRules.ApplyOnce(world,healer,target,false,null,"blocked",room) && target.down &&
                !world.completedTransactions.Contains("blocked") && InventoryJournal.Count(healer,"heal")==2,
                "Хост отклоняет поднятие сквозь стену без расхода и транзакции.");

            var platform=new DynamicSurfaceFrame();
            platform.ReconcileIds(new[]{new DynamicSurfaceSpec("lift",false,false,"metal","metal")});
            var bounds=new Rect(wall.x,wall.y,wall.width,wall.height);
            platform.SetPose("lift",bounds,bounds,0,0,1,false);
            room.platforms=new[]{floor};room.dynamicSurfaces=platform;
            Require(!HealingRules.RevivalPathClear(room,healer,target),"Сплошная подвижная опора перекрывает путь.");
            room.dynamicSurfaces=null;
            wall.oneWay=true;room.platforms=new[]{floor,wall};
            Require(HealingRules.RevivalPathClear(room,healer,target),"Односторонняя опора пропускает проверку видимости.");

            var door=new DoorState {id="gate",definition="grate",level="hub",surfaceTexture="metal",open=true,
                x=wall.x,y=wall.y,width=wall.width,height=wall.height};
            world.doors.Add(door);
            void Compose()=>room.platforms=DoorGeometry.Compose(new[]{floor},world.doors,"hub");
            Compose();Aim();
            Require(focus.Target.kind==InteractionKind.Revival && !timer.Step(healer,target,true,1.4f,room) &&
                timer.Progress>0,"Открытая дверь и пол под ногами позволяют начать поднятие.");
            door.open=false;Compose();Aim();
            Require(focus.Target.kind!=InteractionKind.Revival && !timer.Step(healer,target,true,.2f,room) && timer.Progress==0,
                "Закрытая во время удержания решётка отменяет прогресс.");
            Require(!HealingRules.ApplyOnce(world,healer,target,false,null,"closed",room) && target.down &&
                InventoryJournal.Count(healer,"heal")==2,"Актуальная геометрия хоста блокирует завершение сквозь закрытую дверь.");
            door.open=true;Compose();
            Require(!timer.Step(healer,target,true,.2f,room) && timer.Step(healer,target,true,1.4f,room),
                "После открытия двери требуется новое полное удержание.");
            Require(HealingRules.ApplyOnce(world,healer,target,false,null,"clear",room) && !target.down &&
                InventoryJournal.Count(healer,"heal")==1,"Открытый путь позволяет поднять союзника и списывает одну аптечку.");
            door.open=false;Compose();
            Require(HealingRules.ApplyOnce(world,healer,target,false,null,"clear",room) && InventoryJournal.Count(healer,"heal")==1,
                "Повтор подтверждённой транзакции после закрытия двери остаётся идемпотентным.");
        }
        private static void CheckSelectedStack()
        {
            var actor=new ActorState {id="selected-heal",player=true,connected=true,health=20};
            var first=SharedInventory.Create("heal",2);var chosen=SharedInventory.Create("heal",3);
            var ammo=SharedInventory.Create("p10",5);
            actor.inventory.Add(first);actor.inventory.Add(chosen);actor.inventory.Add(ammo);
            Require(HealingRules.Apply(actor,actor,true,chosen.id) && chosen.count==2 && first.count==2 && actor.health==55,
                "Лечение списало первую стопку вместо выбранной");
            actor.inventoryReservations[chosen.id]=2;
            foreach(var id in new[]{chosen.id,"missing-stack",ammo.id,""})
                Require(!HealingRules.Apply(actor,actor,true,id) && actor.health==55 && first.count==2 && chosen.count==2 && ammo.count==5,
                    "Недоступная выбранная стопка заменилась другой или изменила здоровье: "+id);
            Require(HealingRules.Apply(actor,actor,true) && first.count==1 && actor.health==90,
                "Лечение горячей клавишей перестало выбирать доступную стопку");
            Debug.Log("Выбранная аптечка: PASS — точный расход, резерв/чужой тип/нет ID без подмены, горячая клавиша.");
        }
        private static void Require(bool value,string reason) {if(!value) throw new InvalidOperationException("RevivalChecks: "+reason);}
    }
}
