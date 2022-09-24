import { Button, Collapse, IconButton, Table, TableBody, TableCell, TableHead, TableRow, Typography } from '@mui/material';
import { FC, useEffect } from 'react';

import * as React from 'react';
import KeyboardArrowDownIcon from '@mui/icons-material/KeyboardArrowDown';
import KeyboardArrowUpIcon from '@mui/icons-material/KeyboardArrowUp';
import { useAppDispatch, useAppSelector } from '../store/Hooks';
import { useNavigate } from 'react-router-dom';
import { joinGame } from '../store/Game';

const GameItem: FC<{ token: string }> = ({ token }) => {
  const [open, setOpen] = React.useState(false);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const game = useAppSelector(state => state.games.byId[token])
  const joinedGame = useAppSelector(state => state.game);

  const handlePlay = () => {
    dispatch(joinGame(token));
  }

  useEffect(() => {
    if (joinedGame.hasGame) {
      navigate('/play');
    }
  }, [joinedGame.hasGame]);

  return (
    <React.Fragment>
      <TableRow
        sx={{ '& > *': { borderBottom: 'unset' } }}
        onClick={() => setOpen(!open)}
      >
        <TableCell>
          <IconButton
            aria-label="expand row"
            size="small"
          >
            {open ? <KeyboardArrowUpIcon /> : <KeyboardArrowDownIcon />}
          </IconButton>
        </TableCell>
        <TableCell component="th" scope="row">
          {game.name}
        </TableCell>
        <TableCell align="right">{game.players && game.players.find(player => player.isHost)!.username}</TableCell>
        <TableCell align="right">{game.description}</TableCell>
        <TableCell align="right">Normal</TableCell>
      </TableRow>
      <TableRow>
        <TableCell style={{ paddingBottom: 0, paddingTop: 0 }} colSpan={6}>
          <Collapse in={open} timeout="auto" unmountOnExit>
            <div style={{ margin: 1 }}>
              <Typography variant="h6" gutterBottom component="div">
                History
              </Typography>
              <Table size="small" aria-label="purchases">
                <TableHead>
                  <TableRow>
                    <TableCell>Date</TableCell>
                    <TableCell>Customer</TableCell>
                    <TableCell align="right">Amount</TableCell>
                    <TableCell align="right">Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  <TableRow key={ game.token }>
                    <TableCell component="th" scope="row">
                      { game.name }
                    </TableCell>
                    <TableCell>{game.description}</TableCell>
                    <TableCell align="right">{game.players.find(player => player.isHost)!.username}</TableCell>
                    <TableCell align="right">
                      <Button onClick={handlePlay}>
                        Play
                      </Button>
                    </TableCell>
                  </TableRow>
                </TableBody>
              </Table>
            </div>
          </Collapse>
        </TableCell>
      </TableRow>
    </React.Fragment>
  );
}

export default GameItem;