import { Divider, Paper, Slider, styled, ToggleButton, ToggleButtonGroup } from '@mui/material';
import FirstPageIcon from '@mui/icons-material/FirstPage';
import ChevronLeftIcon from '@mui/icons-material/ChevronLeft';
import PlayArrowIcon from '@mui/icons-material/PlayArrow';
import PauseIcon from '@mui/icons-material/Pause';
import ChevronRightIcon from '@mui/icons-material/ChevronRight';
import LastPageIcon from '@mui/icons-material/LastPage';
import ThreeDRotationIcon from '@mui/icons-material/ThreeDRotation';
import FormatItalicIcon from '@mui/icons-material/FormatItalic';
import FormatUnderlinedIcon from '@mui/icons-material/FormatUnderlined';
import FormatColorFillIcon from '@mui/icons-material/FormatColorFill';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import { FC, useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { giveUp, passTurn, setStep, toggleCameraMode } from '../store/Game';

const StyledToggleButtonGroup = styled(ToggleButtonGroup)(({ theme }) => ({
  '& .MuiToggleButtonGroup-grouped': {
    margin: theme.spacing(0.5),
    border: 0,
    '&.Mui-disabled': {
      border: 0,
    },
    '&:not(:first-of-type)': {
      borderRadius: theme.shape.borderRadius,
    },
    '&:first-of-type': {
      borderRadius: theme.shape.borderRadius,
    },
  },
}));

const Timeline: FC = () => {
  const dispatch = useAppDispatch();
  const cameraMode = useAppSelector(state => state.game.controls.mode);
  const turns = useAppSelector(state => state.game.turns);
  const step = useAppSelector(state => state.game.step);

  const [turn, setTurn] = useState(turns.length);
  const [isPlaying, setPlaying] = useState(false);

  useEffect(() => {
    if (isPlaying) {
      const increaseStep = () => {
        const nextTurn = Math.min(turn + 1, turns.length);

        if (nextTurn >= turns.length) {
          setPlaying(false);
				}

        dispatch(setStep(nextTurn));
        setTurn(nextTurn);
			}

      const playInterval = setInterval(increaseStep, 2000)

      return () => {
        clearInterval(playInterval);
      }
    }
  }, [isPlaying, turn]);

  const handleCameraModeToggle = () => {
    dispatch(toggleCameraMode());
  }

  const handleGiveUpButton = () => {
    dispatch(giveUp());
  }

  const handlePass = () => {
    dispatch(passTurn());
  }

  const handleTurnChange = (_, value: number | number[], activeThumb: number) => {
    var currentValue = value as number;

    setTurn(currentValue);
  }

  const handleTurnBegin = () => {
    setTurn(0);

    dispatch(setStep(0));
  }

  const handleTurnEnd = () => {
    setTurn(turns.length);

    dispatch(setStep(turns.length));
	}

  const handleTurnStepBackwards = () => {
    const currentStep = Math.max(turn - 1, 0);

    setTurn(currentStep);

    dispatch(setStep(currentStep));
  }

  const handleTurnStepForwards = () => {
    const currentStep = Math.min(turn + 1, turns.length);

    setTurn(currentStep);

    dispatch(setStep(currentStep));
  }

  const handleTimelineChange = (_, value: number | number[]) => {
    var currentValue = value as number;

    dispatch(setStep(currentValue));
  }

  const handlePlaychange = () => {
    setPlaying(!isPlaying);
	}

	return (
    <Paper
      elevation={5}
      sx={{
        position: 'absolute',
        bottom: '50px',
        left: '50%',
        transform: 'translate(-50%)',
        display: 'flex',
        border: (theme) => `1px solid ${theme.palette.divider}`,
        flexWrap: 'wrap',
        zIndex: (theme) => theme.zIndex.fab,
        pointerEvents: 'auto',
      }}
    >
      <StyledToggleButtonGroup
        size="small"
        exclusive
        aria-label="text alignment"
      >
        <ToggleButton
          value="first"
          aria-label="left aligned"
          onClick={handleTurnBegin}
        >
          <FirstPageIcon />
        </ToggleButton>
        <ToggleButton
          value="prev"
          aria-label="centered"
          onClick={handleTurnStepBackwards}
        >
          <ChevronLeftIcon />
        </ToggleButton>
        <ToggleButton
          sx={{
            border: '1px solid #ffffff1f !important',
            borderRadius: '50% !important',
            boxShadow: '0px 3px 5px -1px #0003,0px 5px 8px 0px #00000024,0px 1px 14px 0px #0000001f',
            transform: 'scale(1.2)'
          }}
          color="primary"
          value="play"
          aria-label="right aligned"
          onClick={handlePlaychange}
        >
          {isPlaying ? (<PauseIcon />) : (<PlayArrowIcon />)}
        </ToggleButton>
        <ToggleButton
          value="next"
          aria-label="justified"
          onClick={handleTurnStepForwards}
        >
          <ChevronRightIcon />
        </ToggleButton>
        <ToggleButton
          value="last"
          aria-label="justified"
          onClick={handleTurnEnd}
        >
          <LastPageIcon  />
        </ToggleButton>
      </StyledToggleButtonGroup>
      <Divider flexItem orientation="vertical" sx={{ mx: 0.5, my: 1 }} />
      <ToggleButton
        value=""
        selected={cameraMode === 'perspective'}
        aria-label="bold"
        onClick={handleCameraModeToggle}
      >
        <ThreeDRotationIcon />
      </ToggleButton>
      <Divider flexItem orientation="vertical" sx={{ mx: 0.5, my: 1 }} />
      <StyledToggleButtonGroup
        size="small"
        aria-label="text formatting"
      >
        <ToggleButton value="italic" aria-label="italic">
          <FormatItalicIcon />
        </ToggleButton>
        <ToggleButton
          value="underlined"
          aria-label="underlined"
          onClick={handlePass}
        >
          <FormatUnderlinedIcon />
        </ToggleButton>
        <ToggleButton
          value="color"
          aria-label="color"
          onClick={handleGiveUpButton}
        >
          <FormatColorFillIcon />
          <ArrowDropDownIcon />
        </ToggleButton>
      </StyledToggleButtonGroup>
      <Slider
        sx={{
          pt: 1,
          pb: 0,
          '.MuiSlider-rail, .MuiSlider-track': {
            height: '8px'
          },
        }}
        aria-label="Sets"
        value={turn}
        valueLabelDisplay="auto"
        step={1}
        marks
        min={0}
        max={turns.length}
        onChange={handleTurnChange}
        onChangeCommitted={handleTimelineChange}
      />
    </Paper>
	);
}

export default Timeline;