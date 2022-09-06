import { FC } from 'react';
import Playfield from '../Playfield';
import { useAppSelector } from '../store/Hooks';
import Staging from './Staging';

const Game: FC = () => {
	const players = useAppSelector(state => state.game.players);

	return players && players.length > 1
		? (
			<Playfield/>			
		)
		: (
			<Staging/>
		)
}

export default Game;