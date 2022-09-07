import { FC, useEffect } from 'react';
import { useDispatch } from 'react-redux';
import Playfield from './Playfield';
import { useAppSelector } from '../store/Hooks';
import { changeWorldSettings } from '../store/World';
import Staging from './Staging';

const Game: FC = () => {
	const dispatch = useDispatch();

	const players = useAppSelector(state => state.game.players);

	useEffect(() => {
		dispatch(changeWorldSettings({
			interactable: false,
			orbitControl: false,
			orbitAutoRotate: false,
			countrySelect: false,
			countrySelectMaxCount: 1
		}))
	});

	return players && players.length > 1
		? (
			<Playfield/>			
		)
		: (
			<Staging/>
		)
}

export default Game;