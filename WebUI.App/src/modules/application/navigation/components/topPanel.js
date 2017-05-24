import {connect} from 'react-redux';
import {componentDidMount} from 'react-lifecycle-decorators';

import {Navbar, Nav, NavItem} from 'react-bootstrap';
import { LinkContainer } from 'react-router-bootstrap';

let TopPanel = ({router}) => (
    <Navbar inverse collapseOnSelect>
        <Navbar.Header>
            <Navbar.Brand>
                <LinkContainer to="/App">
                    <NavItem>ASP.NET Deploy</NavItem>
                </LinkContainer>
            </Navbar.Brand>
            <Navbar.Toggle />
        </Navbar.Header>
        <Navbar.Collapse>
            <Nav>
                <LinkContainer to="/App/Sources">
                    <NavItem eventKey={1}>Sources</NavItem>
                </LinkContainer>
            </Nav>
        </Navbar.Collapse>
    </Navbar>
);

TopPanel = componentDidMount(
    (props) => {
    }
)(TopPanel);

export default connect(
    (state) => {
        return state;
    }
)(TopPanel);