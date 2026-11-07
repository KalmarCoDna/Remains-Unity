#!/usr/bin/env python3
"""Run existing production graphical probes as two isolated native Player processes in both host modes."""
import argparse, datetime, hashlib, json, os, pathlib, subprocess, time

p=argparse.ArgumentParser()
p.add_argument('--player',required=True,type=pathlib.Path)
p.add_argument('--output',required=True,type=pathlib.Path)
p.add_argument('--modes',nargs='+',choices=['on','off'],default=['on','off'])
p.add_argument('--scenarios',nargs='+',default=['appearance','door','melee','grenade','revival','telekinesis','telekinesis-container','telekinesis-pause','projectile-handoff','npc','container','pause','prop-body','shortcuts'])
p.add_argument('--port',type=int,default=19440)
p.add_argument('--timeout',type=int,default=180)
a=p.parse_args();a.player=a.player.resolve();a.output=a.output.resolve();a.output.mkdir(parents=True,exist_ok=True)
if not a.player.is_file():p.error('Player does not exist')
assembly=a.player.parent/(a.player.stem+'_Data')/'Managed/Assembly-CSharp.dll'
metadata={'utc':datetime.datetime.now(datetime.timezone.utc).isoformat(),'player':str(a.player),'assembly_sha256':hashlib.sha256(assembly.read_bytes()).hexdigest(),'runs':[]}
(a.output/'summary.json').write_text(json.dumps(metadata,ensure_ascii=False,indent=2))
for mode in a.modes:
 for scenario in a.scenarios:
  directory=a.output/(scenario+'-'+mode)
  directory.mkdir(exist_ok=False)
  port=a.port+len(metadata['runs']);processes=[];streams=[];reports=[]
  try:
   for index in range(2):
    role='host' if index==0 else 'client';character='session-host' if index==0 else 'session-client-1'
    report=directory/(role+'-report.json');reports.append(report)
    probe=scenario+'-'+role
    if scenario in ['contact-audio-props','contact-audio-targets','prop-body','door','shortcuts']:
     probe=scenario.replace('contact-audio-','contact-audio-frames-')+'-'+role if scenario.startswith('contact-audio-') else scenario+'-frames-'+role
    command=[str(a.player),'-batchmode','-force-glcore','-job-worker-count','2','-screen-width','1920','-screen-height','1080','-screen-fullscreen','0','-logFile',str(directory/(role+'.log')),'--role',role,'--character',character,'--save-dir',str(directory/(role+'-save')),'--smoke',probe,'--probe-peers','2','--probe-fps','60','--report',str(report),'--port',str(port),'--host-self-ticks',mode]
    (directory/(role+'-command.json')).write_text(json.dumps(command,ensure_ascii=False,indent=2))
    stream=(directory/(role+'-stdout.log')).open('w');streams.append(stream)
    processes.append(subprocess.Popen(command,stdout=stream,stderr=subprocess.STDOUT))
    if index==0:time.sleep(2)
   deadline=time.monotonic()+a.timeout
   while any(process.poll() is None for process in processes) and time.monotonic()<deadline:time.sleep(.5)
   for process in processes:
    if process.poll() is None:process.terminate()
   for process in processes:
    try:process.wait(timeout=10)
    except subprocess.TimeoutExpired:process.kill();process.wait()
   results=[]
   for role,report,process in zip(['host','client'],reports,processes):
    data=json.loads(report.read_text()) if report.exists() else {'success':False,'detail':'Missing report'}
    data.update(role=role,exit_code=process.returncode)
    results.append(data)
   passed=all(r.get('success') and r['exit_code']==0 for r in results)
   result={'mode':mode,'scenario':scenario,'success':passed,'directory':str(directory),'reports':results}
  except Exception as error:
   result={'mode':mode,'scenario':scenario,'success':False,'error':str(error),'directory':str(directory)}
  finally:
   for process in processes:
    if process.poll() is None:process.kill();process.wait()
   for stream in streams:stream.close()
  metadata['runs'].append(result)
  (a.output/'summary.json').write_text(json.dumps(metadata,ensure_ascii=False,indent=2))
  print(json.dumps({'mode':mode,'scenario':scenario,'success':result['success'],'directory':str(directory)}),flush=True)
raise SystemExit(0 if all(r['success'] for r in metadata['runs']) else 1)
