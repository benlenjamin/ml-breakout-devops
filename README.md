## Dev environment setup

You'll need to install the mlagents python library:
1. Install conda: https://docs.conda.io/projects/conda/en/latest/user-guide/install/index.html. Conda allows you to manage multiple python versions and Python virtual environments (note: not virtual machines) on a single machine.
2. Create a virtual environment for mlagents, then activate it. Run, `conda create -n mlagents python=3.10.12 && conda activate mlagents` If you restart your terminal, you’ll need to re-run the `conda activate mlagents` command.
(Make sure to run the rest of these commands in that same terminal session, where the conda environment is activated)
3. (windows only) install pytorch into your virtual env: pip3 install torch~=2.2.1 --index-url https://download.pytorch.org/whl/cu121
4. Install the mlagents python package: python -m pip install mlagents==1.1.0


## How to train new models (the recommended way)

Do this to update the models for the "easy", "medium", and "hard" difficulty levels.

1. Start the conda environment: `conda activate mlagents`
2. Enter the Assets directory: `cd Assets/`
2. Start this script: `python3 train_models.py`  
3. When the script outputs `[INFO] Listening on port 5004. Start training by pressing the Play button in the Unity Editor.` 
   Go to the "18PlayerAITrainer" scene in Unity, and press play.
4. (wait for the script to finish, about 10 minutes)

When the script is done, it should have created new model snapshot (.onnx) files in the 
Assets/agent-models directory. These files are the trained models for the 3 AI difficulties and will
automatically be loaded by the Unity game the next time you run or built it.

## How to train a new model (the hard way)

If you want more control over the mlagents-learn options, you can invoke it directly using the following steps:

1. Start the conda environment: `conda activate mlagents`
2. Enter the Assets directory: `cd Assets/`
3. Run `mlagents-learn agent-training-configs/config_v0.yaml --run-id=my_run_id --train` Change `my_run_id` to a name for your model. That script should run, then get to a point where it says it's waiting for Unity.
4. In Unity, open up the `18PlayerAITrainer` scene and press Play
5. (At this point, the ML Agents script should take control of the game and start playing through a bunch of training episodes. This can take about 10 minutes to run)

When that's finished, there should be some new files in the Assets/results/ directory for your model. In the next step, we'll tell the game to use that new model.

7. Open up the `MainMenu` Scene.
8. Open `MainManager` in the inspector.
9. Set one of the `Difficulty Models` to the `Paddle.onnx` file your training just created.

To test out your new model, the easiest way is:

1. Open up the `MainMenu` Scene and press Play
2. Select the difficulty level your model is assigned to
3. Select the "AI Player" mode, then watch your model in action!


## Debugging

Here are some handy tips for solving common problems:

* If the Unity editor crashes while you're trying to play a scene that includes an agent running in inference mode, check that the model you're using was trained using the same Vector Observation Stack Size as what's currently being used in your code. If there's a mismatch, that'll likely result in a crash (based on my experience)

## Example Screenshots
<p align="center">
Title Screen: <br/>
<img src="https://imgur.com/a/TKK7rFd" height="80%" width="80%" alt="ML-Breakout Title Screen"/>
<br />
<br />
<p align="center">
Main Menu: <br/>
<img src="https://imgur.com/f1Y4FqW" height="80%" width="80%" alt="ML-Breakout Main Menu"/>
<br />
<br />
<p align="center">
Gameplay: <br/>
<img src="https://imgur.com/Cw0kxzG" height="80%" width="80%" alt="ML-Breakout Gameplay"/>
<br />
<br />
<p align="center">
Pause Menu: <br/>
<img src="https://imgur.com/FNEWwT8" height="80%" width="80%" alt="ML-Breakout Pause Menu"/>
<br />
<br />
<p align="center">
Volume Menu: <br/>
<img src="https://imgur.com/FvKUX1B" height="80%" width="80%" alt="ML-Breakout Volume Menu"/>
<br />
<br />

</p>
