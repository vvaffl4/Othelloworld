import { Avatar, Box, Stack, Typography, useTheme } from '@mui/material';
import { FC } from 'react';
import PlayerInGame, { Color } from '../model/PlayerInGame';
import CircleIcon from '@mui/icons-material/Circle';
import CircleOutlinedIcon from '@mui/icons-material/CircleOutlined';

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
      <Stack
        direction="column"
        alignItems="center"
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
        <Avatar alt="country-flag">
          <span
            className={`fi fi-${playerInGame.player.countryCode.toLowerCase()} fis`}
            style={{
              display: 'inline-block',
              fontSize: 36,
              width: '100%',
              height: '100%'
            }}
          />
        </Avatar>
        <Box
          component="div"
        >
          {playerInGame.color === Color.white
            ? (<CircleIcon />)
            : (<CircleOutlinedIcon />)
          }
        </Box>
        <Typography
          variant="h5"
        >
          {playerInGame.player.username}
        </Typography>
      </Stack>
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
      <Stack
        direction="column"
        alignItems="center"
      >
      <Typography
        variant="h5"
        sx={{
          pt: 1
				}}
      >
        {playerInGame.player.username}
      </Typography>
        <Box
          component="div"
        >
        {playerInGame.color === Color.white
          ? (<CircleIcon />)
          : (<CircleOutlinedIcon />)
        }
      </Box>
        <Avatar alt="country-flag">
        <span
          className={`fi fi-${playerInGame.player.countryCode.toLowerCase()} fis`}
          style={{
            display: 'inline-block',
            fontSize: 36,
            width: '100%',
            height: '100%'
          }}
        />
      </Avatar>
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
      </Stack>
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