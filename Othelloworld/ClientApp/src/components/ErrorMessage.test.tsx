// react-testing-library renders your components to document.body,
// this adds jest-dom's custom assertions
import '@testing-library/jest-dom';

import { act, render, screen, waitForElementToBeRemoved } from '@testing-library/react';
import userEvent from '@testing-library/user-event'

import ErrorMessage from './ErrorMessage';

//jest.mock('react-transition-group', () => {
//  const FakeTransition = jest.fn(({ children }) => children)
//  const FakeCSSTransition = jest.fn(props =>
//    props.in ? <FakeTransition>{props.children}</FakeTransition> : null,
//  )
//  return { CSSTransition: FakeCSSTransition, Transition: FakeTransition }
//})

describe('ErrorMessage Tests', () => {
  test('renders welcome message', () => {
    act(() => {
      render(<ErrorMessage />);
    });
    expect(screen.getByText('You\'ve received an error. Oh no!')).toBeInTheDocument();
  });

  test('click on close', async () => {
    act(() => {
      render(<ErrorMessage />);
		})

    act(() => {
      userEvent.click(screen.getByText('Close'));
    });

    await waitForElementToBeRemoved(() => screen.queryByText('You\'ve received an error. Oh no!'));

    expect(screen.queryByText('You\'ve received an error. Oh no!')).not.toBeInTheDocument();
	})
});