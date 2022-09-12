import { Box, Typography, useTheme } from '@mui/material';
import { FC } from 'react';
import PlayerInGame, { Color } from '../model/PlayerInGame';

interface ScoreSideProps {
  playerInGame: PlayerInGame;
  score: number;
}

const ScoreBottomLeft: FC<ScoreSideProps> = ({ playerInGame, score }) => {
  const theme = useTheme();

  return (
    <Box
      component="div"
      sx={{
        position: 'absolute',
        bottom: 0,
        left: 0,
        width: '100px',
        height: '100px',
        zIndex: 3,
        background: theme.palette.background.paper,
        ':before': {
          content: '" "',
          position: 'absolute',
          top: 0,
          right: 0,
          width: '150px',
          height: '150px',
          transform: 'rotate(45deg)',
          transformOrigin: '100% 0%',
          backgroundColor: theme.palette.background.paper,
          zIndex: -1
        }
      }}
    >
      <Typography
        sx={{
          mt: '-120px',
          pl: '25px',
          fontSize: 72,
          fontWeight: 'bold',
          textShadow: `-7px 7px ${theme.palette.primary[theme.palette.mode]}`
        }}
      >
        { score }
      </Typography>
      <Typography>
        { playerInGame.color === Color.white
          ? 'White'
          : 'Black'
        }
      </Typography>
      <Typography
        variant="h5"
      >
        {playerInGame.player.username}
      </Typography>
    </Box>
  );
}

const ScoreTopRight: FC<ScoreSideProps> = ({ playerInGame, score }) => {
  const theme = useTheme();

  return (
    <Box
      component="div"
      sx={{
        position: 'absolute',
        top: '56px',
        right: 0,
        width: '100px',
        height: '100px',
        zIndex: 3,
        background: theme.palette.background.paper,
        ':before': {
          content: '" "',
          position: 'absolute',
          bottom: 0,
          keft: 0,
          width: '150px',
          height: '150px',
          transform: 'rotate(45deg)',
          transformOrigin: '0% 100%',
          backgroundColor: theme.palette.background.paper,
          zIndex: -1
        }
      }}
    >
      <Typography
        variant="h5"
      >
        {playerInGame.player.username}
      </Typography>
      <Typography>
        { playerInGame.color === Color.white
          ? 'White'
          : 'Black'
        }
      </Typography>
      <Typography
        sx={{
          mt: '40px',
          pr: '25px',
          fontSize: 72,
          fontWeight: 'bold',
          textShadow: `7px -7px ${theme.palette.primary[theme.palette.mode]}`
        }}
      >
        { score }
      </Typography>
    </Box>
  );
}

interface ScoreProps extends ScoreSideProps {
  side: 'left' | 'right'
}

const Score: FC<ScoreProps> = ({ side, playerInGame, score }) => {
  return (
    side === 'left'
      ? <ScoreBottomLeft playerInGame={playerInGame} score={score} />
      : <ScoreTopRight playerInGame={playerInGame} score={score} />
	);
}

export default Score;