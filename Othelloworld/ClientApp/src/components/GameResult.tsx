import { Button, Dialog, DialogActions, DialogContent, DialogContentText, DialogTitle } from '@mui/material';
import { FC, useEffect } from 'react';
import { shallowEqual } from 'react-redux';
import { GameStatus } from '../model/Game';
import { useAppSelector } from '../store/Hooks';

const GameResult: FC = () => {
  const game = useAppSelector(state => state.game, shallowEqual);

	return (
    <Dialog
      open={game.status == GameStatus.Finished}
      aria-labelledby="alert-dialog-title"
      aria-describedby="alert-dialog-description"
    >
      <DialogTitle id="alert-dialog-title">
        {"Use Google's location service?"}
      </DialogTitle>
      <DialogContent>
        <DialogContentText id="alert-dialog-description">
          Let Google help apps determine location. This means sending anonymous
          location data to Google, even when no apps are running.
        </DialogContentText>
      </DialogContent>
      <DialogActions>
        <Button>Disagree</Button>
        <Button autoFocus>
          Agree
        </Button>
      </DialogActions>
    </Dialog>
	);
}

export default GameResult;